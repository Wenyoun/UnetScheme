using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace Zyq.Game.Base
{
    public interface IKcpConnect
    {
        void OnKcpConnect(IChannel con);

        void OnKcpDisconnect(IChannel con);
    }

    public class KcpUdpServer : IDisposable
    {
        private Socket udp;
        private bool isDispose;
        private IKcpConnect kcpConnect;
        private ConcurrentDictionary<long, ServerChannel> channels;

        public KcpUdpServer()
        {
            udp = null;
            isDispose = false;
            kcpConnect = null;
            channels = new ConcurrentDictionary<long, ServerChannel>();
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Bind(int port, IKcpConnect connect)
        {
            kcpConnect = connect;
            udp.Bind(new IPEndPoint(IPAddress.Any, port));
            ThreadPool.QueueUserWorkItem(UpdateKcpLooper);
            ThreadPool.QueueUserWorkItem(RecvUdpDataLooper);
            ThreadPool.QueueUserWorkItem(RecvKcpDataLooper);
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;

                if (channels != null)
                {
                    IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();
                    while (its.MoveNext())
                    {
                        its.Current.Value.Dispose();
                    }

                    channels.Clear();
                }

                if (udp != null)
                {
                    udp.Dispose();
                }
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                uint startConvId = 10000;
                byte[] buffer = new byte[KcpHelper.Length];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                while (!isDispose)
                {
                    if (!udp.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    CheckDispose();

                    int count = udp.ReceiveFrom(buffer, SocketFlags.None, ref remote);
                    long conId = CptConId(remote);
                    ServerChannel channel;
                    if (!channels.TryGetValue(conId, out channel))
                    {
                        if (count == 4)
                        {
                            uint flag = KcpHelper.Decode32u(buffer, 0);
                            if (flag == KcpHelper.ConnectFlag)
                            {
                                uint conv = startConvId++;
                                channel = new ServerChannel(new KcpConn(conId, conv, udp,
                                    remote.Create(remote.Serialize())));
                                if (channels.TryAdd(channel.ChannelId, channel))
                                {
                                    KcpHelper.Encode32u(buffer, 0, KcpHelper.ConnectFlag);
                                    KcpHelper.Encode32u(buffer, 4, conv);
                                    channel.Send(buffer, 0, 8);
                                }
                            }
                        }
                    }
                    else if (count > 24)
                    {
                        channel.Input(buffer, 0, count);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void UpdateKcpLooper(object obj)
        {
            try
            {
                List<ServerChannel> removeChannels = new List<ServerChannel>();
                ServerDataProcessingCenter process = new ServerDataProcessingCenter();

                while (!isDispose)
                {
                    DateTime time = DateTime.Now;
                    IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();
                    while (its.MoveNext())
                    {
                        removeChannels.Clear();
                        ServerChannel channel = its.Current.Value;
                        if (channel.IsClose)
                        {
                            removeChannels.Add(channel);
                        }
                        else
                        {
                            channel.ProcessSendPacket(process);
                            channel.Update(time);
                        }
                    }

                    int length = removeChannels.Count;
                    if (length > 0)
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            ServerChannel removeChannel = removeChannels[i];
                            if (kcpConnect != null)
                            {
                                kcpConnect.OnKcpDisconnect(removeChannel);
                            }

                            removeChannel.Dispose();
                        }
                    }

                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void RecvKcpDataLooper(object obj)
        {
            try
            {
                ServerDataProcessingCenter process = new ServerDataProcessingCenter();
                while (!isDispose)
                {
                    IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();
                    while (its.MoveNext())
                    {
                        ServerChannel channel = its.Current.Value;
                        channel.ProcessRecvPacket(process, kcpConnect);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private long CptConId(EndPoint endPoint)
        {
            return endPoint.GetHashCode() * 100000 + ((IPEndPoint) endPoint).Port;
        }

        private void CheckDispose()
        {
            if (isDispose)
            {
                throw new KcpServerException("KcpUdpServer server already dispose");
            }
        }

        private class KcpServerException : Exception
        {
            public KcpServerException(string message) : base(message)
            {
            }
        }
    }
}
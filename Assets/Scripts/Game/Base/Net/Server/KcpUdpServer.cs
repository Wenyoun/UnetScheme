using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Net.KcpImpl;
using UnityEngine;

namespace Zyq.Game.Base
{
    public interface IKcpConnect
    {
        void OnKcpConnect(IChannel channel);

        void OnKcpDisconnect(IChannel channel);
    }

    public class KcpUdpServer : IDisposable
    {
        private Socket socket;
        private bool isDispose;
        private IKcpConnect connectCallback;
        private ConcurrentDictionary<long, ServerChannel> channels;

        public KcpUdpServer()
        {
            isDispose = false;
            channels = new ConcurrentDictionary<long, ServerChannel>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Bind(int port, IKcpConnect connect)
        {
            CheckDispose();

            connectCallback = connect;
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            
            KcpHelper.CreateThread(UpdateKcpLooper);
            KcpHelper.CreateThread(RecvUdpDataLooper);
            KcpHelper.CreateThread(RecvKcpDataLooper);
        }

        public void Dispose()
        {
            if (isDispose)
            {
                return;
            }

            isDispose = true;

            IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();
            while (its.MoveNext())
            {
                its.Current.Value.Dispose();
            }
            channels.Clear();

            if (socket != null)
            {
                socket.Dispose();
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                uint startConvId = 10000;
                byte[] rawBuffer = new byte[KcpConstants.Length];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

                while (!isDispose)
                {
                    if (!socket.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    CheckDispose();

                    int count = socket.ReceiveFrom(rawBuffer, SocketFlags.None, ref remote);
                    long conId = CptConId(remote);

                    ServerChannel channel;
                    if (!channels.TryGetValue(conId, out channel))
                    {
                        if (count == 4)
                        {
                            uint flag = KcpHelper.Decode32u(rawBuffer, 0);
                            if (flag == KcpConstants.Flag_Connect)
                            {
                                uint conv = startConvId++;
                                channel = new ServerChannel(new KcpConn(conId, conv, socket,
                                    remote.Create(remote.Serialize())));
                                if (channels.TryAdd(channel.ChannelId, channel))
                                {
                                    KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                                    KcpHelper.Encode32u(rawBuffer, 4, conv);
                                    channel.Send(rawBuffer, 0, 8);
                                }
                            }
                        }
                    }
                    else if (count >= Kcp.IKCP_OVERHEAD)
                    {
                        channel.Input(rawBuffer, 0, count);
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
                List<long> removeChannels = new List<long>();
                ServerDataProcessingCenter process = new ServerDataProcessingCenter();

                while (!isDispose)
                {
                    DateTime time = DateTime.Now;
                    IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();

                    while (its.MoveNext())
                    {
                        ServerChannel channel = its.Current.Value;
                        if (channel.IsClose)
                        {
                            removeChannels.Add(channel.ChannelId);
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
                            if (channels.TryRemove(removeChannels[i], out ServerChannel channel))
                            {
                                if (connectCallback != null)
                                {
                                    connectCallback.OnKcpDisconnect(channel);
                                }
                                channel.Dispose();
                            }
                        }
                        removeChannels.Clear();
                    }

                    Thread.Sleep(1);
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
                List<Packet> packets = new List<Packet>();
                ServerDataProcessingCenter process = new ServerDataProcessingCenter();
                ServerHeartbeatProcessing heartbeat = new ServerHeartbeatProcessing();

                while (!isDispose)
                {
                    IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();

                    while (its.MoveNext())
                    {
                        ServerChannel channel = its.Current.Value;
                        channel.ProcessRecvPacket(process, packets, connectCallback, heartbeat);
                        heartbeat.Tick(channel);
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private long CptConId(EndPoint endPoint)
        {
            return endPoint.GetHashCode() * 100000L + ((IPEndPoint) endPoint).Port;
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
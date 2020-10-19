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
        private IKcpConnect connect;
        private volatile bool isDispose;
        private ConcurrentDictionary<long, ServerChannel> channels;

        public KcpUdpServer()
        {
            isDispose = false;
            channels = new ConcurrentDictionary<long, ServerChannel>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Bind(int port, IKcpConnect connect)
        {
            this.connect = connect;
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            KcpHelper.CreateThread(UpdateKcpLooper);
            KcpHelper.CreateThread(RecvUdpDataLooper);
            KcpHelper.CreateThread(RecvKcpDataLooper);
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

                if (socket != null)
                {
                    socket.Dispose();
                }
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
                            if (flag == KcpConstants.ConnectFlag)
                            {
                                uint conv = startConvId++;
                                channel = new ServerChannel(new KcpConn(conId, conv, socket,
                                    remote.Create(remote.Serialize())));
                                if (channels.TryAdd(channel.ChannelId, channel))
                                {
                                    KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.ConnectFlag);
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
                List<ServerChannel> removeChannels = new List<ServerChannel>();
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

                            if (connect != null)
                            {
                                connect.OnKcpDisconnect(removeChannel);
                            }

                            removeChannel.Dispose();
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

                while (!isDispose)
                {
                    IEnumerator<KeyValuePair<long, ServerChannel>> its = channels.GetEnumerator();

                    while (its.MoveNext())
                    {
                        its.Current.Value.ProcessRecvPacket(process, packets, connect);
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
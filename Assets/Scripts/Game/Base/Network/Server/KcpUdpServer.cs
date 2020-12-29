using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Net.KcpImpl;
using UnityEngine;

namespace Nice.Game.Base
{
    public interface IKcpConnect
    {
        void OnKcpConnect(IChannel channel);

        void OnKcpDisconnect(IChannel channel);
    }

    public class KcpUdpServer : IDisposable
    {
        private bool m_Dispose;
        private Socket m_Socket;
        private IKcpConnect m_Connect;
        private ConcurrentDictionary<long, ServerChannel> m_Channels;

        public KcpUdpServer()
        {
            m_Dispose = false;
            m_Channels = new ConcurrentDictionary<long, ServerChannel>();
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Bind(int port, IKcpConnect connect)
        {
            CheckDispose();

            m_Connect = connect;
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));

            KcpHelper.CreateThread(UpdateKcpLooper);
            KcpHelper.CreateThread(RecvUdpDataLooper);
            KcpHelper.CreateThread(RecvKcpDataLooper);
        }

        public void Dispose()
        {
            if (m_Dispose)
            {
                return;
            }

            m_Dispose = true;

            using (IEnumerator<KeyValuePair<long, ServerChannel>> its = m_Channels.GetEnumerator())
            {
                while (its.MoveNext())
                {
                    its.Current.Value.Dispose();
                }
                m_Channels.Clear();
            }

            if (m_Socket != null)
            {
                m_Socket.Dispose();
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                uint startConvId = 10000;
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

                while (!m_Dispose)
                {
                    if (!m_Socket.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    if (m_Dispose)
                    {
                        break;
                    }

                    int count = m_Socket.ReceiveFrom(rawBuffer, SocketFlags.None, ref remote);
                    long conId = remote.GetHashCode();

                    if (!m_Channels.TryGetValue(conId, out ServerChannel channel))
                    {
                        if (count == 4)
                        {
                            uint flag = KcpHelper.Decode32u(rawBuffer, 0);
                            if (flag == KcpConstants.Flag_Connect)
                            {
                                uint conv = startConvId++;
                                EndPoint point = remote.Create(remote.Serialize());
                                channel = new ServerChannel(new KcpConn(conId, conv, m_Socket, point));

                                if (m_Channels.TryAdd(conId, channel))
                                {
                                    KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                                    KcpHelper.Encode32u(rawBuffer, 4, conv);
                                    channel.Send(rawBuffer, 0, 8);
                                    channel.Flush();
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("未创建连接，接收到无效包，len=" + count);
                        }
                    }
                    else if (count > Kcp.IKCP_OVERHEAD)
                    {
                        long pConId = KcpHelper.Decode64(rawBuffer, 0);
                        if (pConId == conId)
                        {
                            channel.Input(rawBuffer, KcpConn.HEAD_SIZE, count - KcpConn.HEAD_SIZE);
                        }
                    }
                    else
                    {
                        Debug.Log("已创建连接，接收到无效包，len=" + count);
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
                List<long> removes = new List<long>();
                ServerDataProcessingCenter process = new ServerDataProcessingCenter();

                while (!m_Dispose)
                {
                    using (IEnumerator<KeyValuePair<long, ServerChannel>> its = m_Channels.GetEnumerator())
                    {
                        while (its.MoveNext())
                        {
                            ServerChannel channel = its.Current.Value;
                            if (!channel.IsClose)
                            {
                                channel.ProcessSendPacket(process);
                            }
                            else
                            {
                                removes.Add(channel.ChannelId);
                            }
                        }
                    }

                    int length = removes.Count;
                    if (length > 0)
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            long channelId = removes[i];
                            if (m_Channels.TryRemove(channelId, out ServerChannel channel))
                            {
                                m_Connect?.OnKcpDisconnect(channel);
                                channel.Dispose();
                            }
                        }
                        removes.Clear();
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

                while (!m_Dispose)
                {
                    using (IEnumerator<KeyValuePair<long, ServerChannel>> its = m_Channels.GetEnumerator())
                    {
                        while (its.MoveNext())
                        {
                            ServerChannel channel = its.Current.Value;
                            if (!channel.IsClose)
                            {
                                channel.ProcessRecvPacket(process, packets, m_Connect);
                            }
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void CheckDispose()
        {
            if (m_Dispose)
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
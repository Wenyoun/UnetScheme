using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Net.KcpImpl;
using UnityEngine;

namespace Nice.Game.Base
{
    public class KcpUdpClient : IDisposable
    {
        #region 连接状态
        public const byte None = 0;
        public const byte Error = 1;
        public const byte Timeout = 2;
        public const byte Success = 3;
        public const byte Connecting = 4;
        #endregion

        private KcpConn m_Con;
        private Socket m_Socket;

        private long m_ConId;
        private byte m_Status;
        private bool m_Dispose;

        private ClientHeartbeatProcessing m_Heartbeat;
        private ConcurrentQueue<Packet> m_SendPackets;
        private ConcurrentQueue<Packet> m_RecvPackets;

        public KcpUdpClient()
        {
            m_ConId = -1;
            m_Status = None;
            m_Dispose = false;

            m_Heartbeat = new ClientHeartbeatProcessing();
            m_SendPackets = new ConcurrentQueue<Packet>();
            m_RecvPackets = new ConcurrentQueue<Packet>();
        }

        public void Connect(string host, int port)
        {
            if (m_Status == Connecting)
            {
                return;
            }

            Clear();
            m_Status = Connecting;

            IPAddress[] addresses = Dns.GetHostAddresses(host);

            if (addresses.Length < 1)
            {
                throw new KcpClientException("不能解析的地址:" + host);
            }

            IPEndPoint endPoint = new IPEndPoint(addresses[0], port);
            m_Socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            KcpHelper.CreateThread(OnConnectLooper, endPoint);
        }

        public void Send(Packet packet)
        {
            if (m_Dispose)
            {
                return;
            }
            m_SendPackets.Enqueue(packet);
        }

        public bool Recv(out Packet packet)
        {
            if (m_Dispose)
            {
                packet = new Packet();
                return false;
            }
            return m_RecvPackets.TryDequeue(out packet);
        }

        public long ConId
        {
            get { return m_ConId; }
        }

        public byte Status
        {
            get { return m_Status; }
        }

        public bool IsConnected
        {
            get { return m_Status == Success; }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (m_Dispose)
                {
                    return;
                }
                m_Dispose = true;
            }

            if (m_Con != null)
            {
                m_Con.SendDisconnect();
            }

            Clear();
        }

        private void OnConnectLooper(object obj)
        {
            try
            {
                int time = 0;
                int timeout = 5000;
                m_Socket.Connect((EndPoint) obj);
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];

                while (!m_Dispose && m_Status == Connecting)
                {
                    int length = KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                    m_Socket.Send(rawBuffer, 0, length, SocketFlags.None);
                    if (!m_Socket.Poll(100000, SelectMode.SelectRead))
                    {
                        time += 100;
                        if (time > timeout)
                        {
                            m_Status = Timeout;
                            break;
                        }
                        continue;
                    }

                    if (m_Dispose || m_Status != Connecting)
                    {
                        break;
                    }

                    int count = m_Socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);

                    if (count == 40)
                    {
                        long conId = KcpHelper.Decode64(rawBuffer, 0);
                        uint flag = KcpHelper.Decode32u(rawBuffer, 32);
                        uint conv = KcpHelper.Decode32u(rawBuffer, 36);

                        if (flag == KcpConstants.Flag_Connect)
                        {
                            m_Con = new KcpConn(conId, conv, m_Socket);
                            m_Con.Input(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size);
                            count = m_Con.Recv(rawBuffer, 0, rawBuffer.Length);

                            if (count == 8)
                            {
                                KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                                KcpHelper.Encode32u(rawBuffer, 4, conv);
                                m_Con.Send(rawBuffer, 0, 8);
                                m_Con.Flush();

                                m_ConId = conId;
                                m_Status = Success;

                                KcpHelper.CreateThread(UpdateKcpLooper);
                                KcpHelper.CreateThread(RecvUdpDataLooper);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_Status = Error;
                Debug.LogError(e.ToString());
            }
        }

        private void UpdateKcpLooper(object obj)
        {
            try
            {
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();

                while (!m_Dispose && m_Status == Success)
                {
                    process.SendPackets(m_Con, m_SendPackets);

                    long time = TimeUtil.Get1970ToNowMilliseconds();
                    m_Con.Update(time);
                    m_Heartbeat.OnUpdate(this, m_Con, time);

                    if (m_Dispose || m_Status != Success)
                    {
                        break;
                    }

                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                m_Status = Error;
                Debug.LogError(e.ToString());
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                List<Packet> packets = new List<Packet>();
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];
                ClientDataProcessingCenter process = new ClientDataProcessingCenter();

                while (!m_Dispose && m_Status == Success)
                {
                    if (!m_Socket.Poll(100000, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    if (m_Dispose || m_Status != Success)
                    {
                        break;
                    }

                    int count = m_Socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);
                    
                    if (count > KcpConstants.Head_Size + 1)
                    {
                        long conId = KcpHelper.Decode64(rawBuffer, 0);
                        if (conId == m_Con.ConId)
                        {
                            byte msgChannel = rawBuffer[KcpConstants.Head_Size];
                            
                            if (msgChannel == MsgChannel.Reliable)
                            {
                                m_Con.Input(rawBuffer, KcpConstants.Head_Size + 1, count - KcpConstants.Head_Size - 1);
                                process.RecvReliablePackets(this, m_Con, packets, m_Heartbeat);
                            }
                            else if (msgChannel == MsgChannel.Unreliable)
                            {
                                process.RecvUnreliablePackets(rawBuffer, KcpConstants.Head_Size + 1, count - KcpConstants.Head_Size - 1, packets, m_Heartbeat);
                            }

                            int length = packets.Count;
                            if (length > 0)
                            {
                                for (int i = 0; i < length; ++i)
                                {
                                    m_RecvPackets.Enqueue(packets[i]);
                                }
                                packets.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_Status = Error;
                Debug.LogError(e.ToString());
            }
        }

        private void KcpConDispose()
        {
            if (m_Con != null)
            {
                m_Con.Dispose();
                m_Con = null;
            }
        }

        private void SocketDispose()
        {
            if (m_Socket != null)
            {
                m_Socket.Dispose();
                m_Socket = null;
            }
        }

        private void Clear()
        {
            KcpConDispose();
            SocketDispose();
            m_Status = None;
            m_SendPackets.Clear();
            m_RecvPackets.Clear();
        }

        private class KcpClientException : Exception
        {
            public KcpClientException(string message) : base(message)
            {
            }
        }
    }
}
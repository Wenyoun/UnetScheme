using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Nice.Game.Base
{
    public class ClientSocket : IDisposable
    {
        #region 连接状态
        public const byte None = 0;
        public const byte Error = 1;
        public const byte Timeout = 2;
        public const byte Success = 3;
        public const byte Connecting = 4;
        #endregion

        private KcpCon m_Kcp;
        private Socket m_Socket;

        private uint m_ConId;
        private byte m_Status;
        private bool m_Dispose;

        private ConcurrentQueue<Packet> m_SendPackets;
        private ConcurrentQueue<Packet> m_RecvPackets;
        private ClientDataProcessingCenter m_Process;
        private ClientHeartbeatProcessing m_Heartbeat;

        public ClientSocket()
        {
            m_Kcp = null;
            m_Socket = null;
            
            m_ConId = 0;
            m_Status = None;
            m_Dispose = false;

            m_SendPackets = new ConcurrentQueue<Packet>();
            m_RecvPackets = new ConcurrentQueue<Packet>();
            m_Process = new ClientDataProcessingCenter();
            m_Heartbeat = new ClientHeartbeatProcessing();
        }

        public void Connect(string host, int port)
        {
            if (IsConnected)
            {
                Logger.Debug("ClientSocket已连接");
                return;
            }

            if (m_Status == Connecting)
            {
                Logger.Debug("ClientSocket正在连接中");
                return;
            }

            m_Status = Connecting;

            IPAddress[] addresses = Dns.GetHostAddresses(host);

            if (addresses.Length < 1)
            {
                throw new KcpClientException($"不能解析的地址:{host}");
            }

            IPEndPoint point = new IPEndPoint(addresses[0], port);
            m_Socket = new Socket(point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            KcpHelper.CreateThread(OnConnectLooper, point);
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

        public uint ConId
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

            if (m_Kcp != null)
            {
                m_Kcp.SendDisconnect();
            }

            m_ConId = 0;
            m_Status = None;
            m_SendPackets.Clear();
            m_RecvPackets.Clear();

            KcpDispose();
            SocketDispose();
        }

        private void OnConnectLooper(object obj)
        {
            try
            {
                int time = 0;
                int timeout = 5000;
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];

                m_Socket.Connect((EndPoint) obj);

                while (!m_Dispose && m_Status == Connecting)
                {
                    int size = KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                    m_Socket.Send(rawBuffer, 0, size, SocketFlags.None);
                    if (!m_Socket.Poll(100000, SelectMode.SelectRead))
                    {
                        time += 100;
                        if (time > timeout)
                        {
                            m_Status = Timeout;
                            RecvStatusPacket();
                            break;
                        }
                        continue;
                    }

                    if (m_Dispose || m_Status != Connecting)
                    {
                        break;
                    }

                    int count = m_Socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);

                    if (count == 37)
                    {
                        uint cid = KcpHelper.Decode32u(rawBuffer, 0);
                        uint flag = KcpHelper.Decode32u(rawBuffer, 29);
                        uint conv = KcpHelper.Decode32u(rawBuffer, 33);

                        if (flag == KcpConstants.Flag_Connect)
                        {
                            m_Kcp = new KcpConClient(cid, conv, m_Socket);
                            m_Kcp.Input(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size);
                            count = m_Kcp.Recv(rawBuffer, 0, rawBuffer.Length);

                            if (count == 8)
                            {
                                KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                                KcpHelper.Encode32u(rawBuffer, 4, conv);
                                m_Kcp.Send(rawBuffer, 0, 8);
                                m_Kcp.Flush();

                                m_ConId = cid;
                                m_Status = Success;
                                RecvStatusPacket();

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
                RecvStatusPacket();
            }
        }

        private void UpdateKcpLooper(object obj)
        {
            try
            {

                while (!m_Dispose && m_Status == Success)
                {
                    m_Process.SendPackets(m_Kcp, m_SendPackets);

                    long time = TimeUtil.Get1970ToNowMilliseconds();
                    m_Kcp.Update(time);
                    m_Heartbeat.OnUpdate(this, m_Kcp, time);

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
                RecvStatusPacket();
            }
        }

        private void RecvUdpDataLooper(object obj)
        {
            try
            {
                const int pollTimeout = 10000;
                List<Packet> packets = new List<Packet>();
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];

                while (!m_Dispose && m_Status == Success)
                {
                    if (!m_Socket.Poll(pollTimeout, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    if (m_Dispose || m_Status != Success)
                    {
                        break;
                    }

                    int count = m_Socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);

                    if (count > KcpConstants.Head_Size)
                    {
                        uint remoteCid = KcpHelper.Decode32u(rawBuffer, 0);
                        if (remoteCid == m_Kcp.ConId)
                        {
                            byte msgChannel = rawBuffer[KcpConstants.Conv_Size];

                            if (msgChannel == MsgChannel.Reliable)
                            {
                                m_Kcp.Input(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size);
                                m_Process.RecvReliablePackets(this, m_Kcp, packets, m_Heartbeat);
                            }
                            else if (msgChannel == MsgChannel.Unreliable)
                            {
                                m_Process.RecvUnreliablePackets(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size, packets, m_Heartbeat);
                            }

                            EnqueuePackets(packets);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_Status = Error;
                RecvStatusPacket();
            }
        }

        private void KcpDispose()
        {
            if (m_Kcp != null)
            {
                m_Kcp.Dispose();
                m_Kcp = null;
            }
        }

        private void SocketDispose()
        {
            if (m_Socket != null)
            {
                m_Socket.Close();
                m_Socket = null;
            }
        }

        private void EnqueuePackets(List<Packet> packets)
        {
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

        private void RecvStatusPacket()
        {
            m_RecvPackets.Enqueue(new Packet(m_Status, null, 0));
        }

        private class KcpClientException : Exception
        {
            public KcpClientException(string message) : base(message)
            {
            }
        }
    }
}
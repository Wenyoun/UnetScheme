using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Nice.Game.Base {
    public class ClientSocket {
        internal const byte Msg_Timeout = 0;
        internal const byte Msg_Error = 1;
        internal const byte Msg_Success = 2;
        internal const byte Msg_Disconnect = 3;

        private const byte None = 0;
        private const byte Success = 1;
        private const byte Connecting = 2;

        private uint m_ConId;
        private bool m_Dispose;
        private volatile byte m_Status;

        private KcpCon m_Kcp;
        private Socket m_Socket;

        private SimpleRegister m_Register;
        private ConcurrentQueue<Packet> m_SendPackets;
        private ConcurrentQueue<Packet> m_RecvPackets;


        public ClientSocket() {
            m_ConId = 0;
            m_Status = None;
            m_Dispose = false;

            m_Kcp = null;
            m_Socket = null;

            m_Register = new SimpleRegister();
            m_SendPackets = new ConcurrentQueue<Packet>();
            m_RecvPackets = new ConcurrentQueue<Packet>();
        }

        public void Dispose() {
            lock (this) {
                if (m_Dispose) {
                    return;
                }
                m_Dispose = true;
            }

            m_ConId = 0;
            m_Status = None;
            m_SendPackets.Clear();
            m_RecvPackets.Clear();

            if (m_Kcp != null) {
                m_Kcp.Dispose();
            }

            if (m_Socket != null) {
                m_Socket.Close();
            }

            if (m_Register != null) {
                m_Register.Dispose();
            }
        }

        public void Disconnect() {
            if (m_Dispose || m_Status != Success) {
                return;
            }

            m_Status = None;
            m_Kcp.SendDisconnect();
        }

        public void Connect(string host, int port) {
            if (m_Dispose) {
                throw new ObjectDisposedException("ClientSocket already dispose!");
            }

            if (IsConnected) {
                Logger.Error($"已连接 {host}:{port}");
                return;
            }

            if (m_Status == Connecting) {
                Logger.Error($"正在连接中 {host}:{port}");
                return;
            }

            m_Status = Connecting;

            IPAddress[] addresses = Dns.GetHostAddresses(host);

            if (addresses.Length < 1) {
                throw new KcpClientException($"不能解析的地址:{host}");
            }

            IPEndPoint point = new IPEndPoint(addresses[0], port);
            m_Socket = new Socket(point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            KcpHelper.CreateThread(OnConnectLooper, point);
        }

        public void Send(Packet packet) {
            if (m_Dispose) {
                return;
            }
            m_SendPackets.Enqueue(packet);
        }

        public bool Recv(out Packet packet) {
            if (m_Dispose) {
                packet = new Packet();
                return false;
            }
            return m_RecvPackets.TryDequeue(out packet);
        }

        public uint ConId {
            get { return m_ConId; }
        }

        public bool IsConnected {
            get { return m_Status == Success; }
        }

        private void OnConnectLooper(object obj) {
            try {
                int time = 0;
                const int tick = 100;
                const int timeout = 5000;
                const int pollTimeout = 10000;
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];

                m_Socket.Connect((EndPoint) obj);

                while (!m_Dispose && m_Status == Connecting) {
                    int size = KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                    m_Socket.Send(rawBuffer, 0, size, SocketFlags.None);

                    if (!m_Socket.Poll(pollTimeout, SelectMode.SelectRead)) {
                        time += tick;
                        if (time >= timeout) {
                            m_Register.Notify(Msg_Timeout);
                            Logger.Error($"connect timeout host={obj}");
                            break;
                        }
                        continue;
                    }

                    if (m_Dispose) {
                        break;
                    }

                    int count = m_Socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);

                    if (count == 37) {
                        uint cid = KcpHelper.Decode32u(rawBuffer, 0);
                        uint flag = KcpHelper.Decode32u(rawBuffer, 29);
                        uint conv = KcpHelper.Decode32u(rawBuffer, 33);

                        if (flag == KcpConstants.Flag_Connect) {
                            m_Kcp = new KcpConClient(cid, conv, m_Socket);
                            m_Kcp.Input(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size);
                            count = m_Kcp.Recv(rawBuffer, 0, rawBuffer.Length);

                            if (count == 8) {
                                KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                                KcpHelper.Encode32u(rawBuffer, 4, conv);
                                m_Kcp.Send(rawBuffer, 0, 8);
                                m_Kcp.Flush();

                                m_ConId = cid;
                                m_Status = Success;
                                m_Register.Notify(Msg_Success);

                                KcpHelper.CreateThread(OnHandleLooper);
                                break;
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Logger.Error("OnConnectLooper: " + e);
                m_Register.Notify(Msg_Error);
            }
        }

        private void OnHandleLooper(object obj) {
            try {
                List<Packet> packets = new List<Packet>();
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];
                ClientDataProcessing dataProcess = new ClientDataProcessing();
                ClientHeartbeatProcessing heartbeat = new ClientHeartbeatProcessing();

                while (!m_Dispose && m_Status == Success) {
                    dataProcess.SendPackets(m_Kcp, m_SendPackets);
                    long time = TimeUtil.Get1970ToNowMilliseconds();
                    m_Kcp.OnUpdate(time);
                    heartbeat.OnUpdate(this, m_Kcp, time);

                    while (m_Socket.Poll(0, SelectMode.SelectRead)) {
                        int count = m_Socket.Receive(rawBuffer, 0, rawBuffer.Length, SocketFlags.None);

                        if (count > KcpConstants.Head_Size) {
                            uint remoteCid = KcpHelper.Decode32u(rawBuffer, 0);
                            if (remoteCid == m_Kcp.ConId) {
                                byte msgChannel = rawBuffer[KcpConstants.Conv_Size];

                                if (msgChannel == MsgChannel.Reliable) {
                                    m_Kcp.Input(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size);
                                    dataProcess.RecvReliablePackets(this, m_Kcp, packets, heartbeat);
                                } else if (msgChannel == MsgChannel.Unreliable) {
                                    dataProcess.RecvUnreliablePackets(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size, packets);
                                }

                                int length = packets.Count;
                                if (length > 0) {
                                    for (int i = 0; i < length; ++i) {
                                        m_RecvPackets.Enqueue(packets[i]);
                                    }
                                    packets.Clear();
                                }
                            }
                        }
                    }

                    Thread.Sleep(5);
                }
            } catch (Exception e) {
                Logger.Error("RecvUdpDataLooper: " + e);
                if (!m_Dispose && m_Status == Success) {
                    m_Register.Notify(Msg_Disconnect);
                }
            } finally {
                Dispose();
            }
        }

        internal void OnUpdate() {
            if (m_Dispose) {
                return;
            }
            m_Register.OnUpdate();
        }

        internal void Register(byte id, Action handler) {
            if (m_Dispose) {
                return;
            }
            m_Register.Register(id, handler);
        }

        private class KcpClientException : Exception {
            public KcpClientException(string message) : base(message) {
            }
        }

        private class SimpleRegister : IDisposable {
            private Action[] m_Handlers;
            private ConcurrentQueue<byte> m_Queue;

            public SimpleRegister() {
                m_Handlers = new Action[4];
                m_Queue = new ConcurrentQueue<byte>();
            }

            public void Dispose() {
                m_Queue = null;
                m_Handlers = null;
            }

            public void Notify(byte id) {
                m_Queue.Enqueue(id);
            }

            public void Register(byte id, Action handler) {
                if (id >= m_Handlers.Length) {
                    return;
                }
                m_Handlers[id] = handler;
            }

            public void OnUpdate() {
                while (m_Queue.TryDequeue(out byte id)) {
                    if (id >= m_Handlers.Length) {
                        continue;
                    }
                    try {
                        Action handler = m_Handlers[id];
                        if (handler != null) {
                            handler.Invoke();
                        }
                    } catch (Exception e) {
                        Logger.Error(e.ToString());
                    }
                }
            }
        }
    }
}
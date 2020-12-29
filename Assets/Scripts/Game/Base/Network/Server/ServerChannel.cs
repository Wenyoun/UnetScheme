using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zyq.Game.Base
{
    public class ServerChannel : AbsChannel
    {
        private bool m_IsClose;
        private bool m_IsDispose;
        private uint m_Conv;
        private long m_ConId;

        private KcpConn m_Con;
        private ServerHeartbeatProcessing m_Heartbeat;
        private ConcurrentQueue<Packet> m_RecvPacketQueue;
        private ConcurrentQueue<Packet> m_SendPacketQueue;

        public ServerChannel(KcpConn con)
        {
            m_Con = con;
            m_Conv = con.Conv;
            m_ConId = con.ConId;

            m_IsClose = false;
            m_IsDispose = false;

            m_Heartbeat = new ServerHeartbeatProcessing();
            m_RecvPacketQueue = new ConcurrentQueue<Packet>();
            m_SendPacketQueue = new ConcurrentQueue<Packet>();
        }

        public override long ChannelId
        {
            get { return m_ConId; }
        }

        public override bool IsConnected
        {
            get { return !m_IsDispose && m_Con.IsConnected; }
        }

        public override void Disconnect()
        {
            if (m_IsDispose)
            {
                return;
            }

            m_IsClose = true;
        }

        public override void Dispatcher()
        {
            if (m_IsDispose)
            {
                return;
            }

            if (m_RecvPacketQueue.TryDequeue(out Packet packet))
            {
                ushort cmd = packet.Cmd;
                ByteBuffer byteBuffer = packet.Buffer;
                if (handlers.TryGetValue(packet.Cmd, out ChannelMessageDelegate handler))
                {
                    handler.Invoke(new ChannelMessage(cmd, byteBuffer));
                }
            }
        }

        public override void Send(ushort cmd, ByteBuffer buffer)
        {
            if (m_IsDispose)
            {
                return;
            }

            m_SendPacketQueue.Enqueue(new Packet(cmd, buffer));
        }

        public override void Dispose()
        {
            lock (this)
            {
                if (m_IsDispose)
                {
                    return;
                }

                m_IsDispose = true;
            }

            base.Dispose();

            m_Con.SendDisconnect();
            m_Con.Dispose();

            m_SendPacketQueue.Clear();
            m_RecvPacketQueue.Clear();
        }

        #region internal method
        internal int Send(byte[] buffer, int offset, int length)
        {
            if (m_IsDispose)
            {
                return -20;
            }

            return m_Con.Send(buffer, offset, length);
        }

        internal int Recv(byte[] buffer, int offset, int length)
        {
            if (m_IsDispose)
            {
                return -20;
            }

            return m_Con.Recv(buffer, offset, length);
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (m_IsDispose)
            {
                return;
            }

            m_Con.Input(buffer, offset, length);
        }

        internal void Flush()
        {
            if (m_IsDispose)
            {
                return;
            }

            m_Con.Flush();
        }

        internal void ProcessSendPacket(ServerDataProcessingCenter process)
        {
            if (m_IsDispose)
            {
                return;
            }

            process.TryParseSendKcpData(this, m_SendPacketQueue);

            m_Con.Update(TimeUtil.Get1970ToNowMilliseconds());
        }

        internal void ProcessRecvPacket(ServerDataProcessingCenter process, List<Packet> packets, IKcpConnect connectCallback)
        {
            if (m_IsDispose)
            {
                return;
            }

            if (process.TryParseRecvKcpData(this, packets, connectCallback, m_Heartbeat))
            {
                m_Heartbeat.UpdateHeartbeat();
                for (int i = 0; i < packets.Count; ++i)
                {
                    m_RecvPacketQueue.Enqueue(packets[i]);
                }
                packets.Clear();
            }

            m_Heartbeat.OnUpdate(this);
        }

        internal void SetConnectedStatus(bool status)
        {
            if (m_IsDispose)
            {
                return;
            }

            m_Con.IsConnected = status;
        }

        internal uint Conv
        {
            get { return m_Conv; }
        }

        internal bool IsClose
        {
            get { return m_IsDispose || m_IsClose; }
        }
        #endregion
    }
}
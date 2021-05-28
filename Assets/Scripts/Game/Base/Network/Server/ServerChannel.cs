using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base
{
    public class ServerChannel : AbsChannel
    {
        private uint m_Conv;
        private uint m_ConId;
        private bool m_Dispose;

        private KcpCon m_Con;
        private ServerHeartbeatProcessing m_Heartbeat;
        private ConcurrentQueue<Packet> m_RecvPackets;
        private ConcurrentQueue<Packet> m_SendPackets;

        public ServerChannel(KcpCon con)
        {
            m_Dispose = false;
            m_Conv = con.Conv;
            m_ConId = con.ConId;

            m_Con = con;
            m_Heartbeat = new ServerHeartbeatProcessing();
            m_RecvPackets = new ConcurrentQueue<Packet>();
            m_SendPackets = new ConcurrentQueue<Packet>();
        }

        public override void Dispose()
        {
            lock (this)
            {
                if (m_Dispose)
                {
                    return;
                }
                m_Dispose = true;
            }

            ClearMsgHandlers();

            m_Con.SendDisconnect();
            m_Con.Dispose();

            m_SendPackets.Clear();
            m_RecvPackets.Clear();
        }

        public override void Disconnect()
        {
            Dispose();
        }

        public override void OnUpdate()
        {
            if (m_Dispose)
            {
                return;
            }

            if (m_RecvPackets.TryDequeue(out Packet p))
            {
                try
                {
                    CallMsgHandler(p.Cmd, p.Buffer);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (m_Dispose)
            {
                return;
            }
            m_SendPackets.Enqueue(new Packet(cmd, buffer, channel));
        }

        internal int RawSend(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -20;
            }
            return m_Con.RawSend(buffer, offset, length);
        }

        internal int Send(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -20;
            }
            return m_Con.Send(buffer, offset, length);
        }

        internal int Recv(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -20;
            }

            return m_Con.Recv(buffer, offset, length);
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return;
            }

            m_Con.Input(buffer, offset, length);
        }

        internal void Flush()
        {
            if (m_Dispose)
            {
                return;
            }

            m_Con.Flush();
        }

        internal void OnUpdate(long time, ServerDataProcessing process, IChannelListener listener)
        {
            if (m_Dispose)
            {
                return;
            }
            process.SendPackets(this, m_SendPackets);
            m_Con.OnUpdate(time);
            m_Heartbeat.OnUpdate(this, listener);
        }

        internal void RecvReliablePackets(ServerDataProcessing process, List<Packet> packets, IChannelListener listener)
        {
            if (m_Dispose)
            {
                return;
            }

            if (process.RecvReliablePackets(this, packets, listener, m_Heartbeat))
            {
                HandlePackets(packets);
            }
        }

        internal void RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, ServerDataProcessing process, List<Packet> packets)
        {
            if (m_Dispose)
            {
                return;
            }

            if (process.RecvUnreliablePackets(rawBuffer, offset, count, packets))
            {
                HandlePackets(packets);
            }
        }

        internal void SetConnectedStatus(bool status)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Con.IsConnected = status;
        }

        internal uint Conv
        {
            get { return m_Conv; }
        }

        internal bool IsDispose
        {
            get { return m_Dispose; }
        }

        public override uint ChannelId
        {
            get { return m_ConId; }
        }

        public override bool IsConnected
        {
            get { return !m_Dispose && m_Con.IsConnected; }
        }

        private void HandlePackets(List<Packet> packets)
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
    }
}
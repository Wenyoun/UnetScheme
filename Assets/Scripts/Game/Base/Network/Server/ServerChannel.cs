﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base {
    public class ServerChannel : AbsChannel {
        private bool m_Dispose;
        private uint m_Conv;
        private uint m_ConId;

        private KcpCon m_Con;
        private ServerHeartbeatProcessing m_Heartbeat;
        private ConcurrentQueue<Packet> m_RecvPackets;
        private ConcurrentQueue<Packet> m_SendPackets;

        public ServerChannel(KcpCon con) {
            m_Con = con;
            m_Conv = con.Conv;
            m_ConId = con.ConId;

            m_Dispose = false;

            m_Heartbeat = new ServerHeartbeatProcessing();
            m_RecvPackets = new ConcurrentQueue<Packet>();
            m_SendPackets = new ConcurrentQueue<Packet>();
        }

        public override uint ChannelId {
            get { return m_ConId; }
        }

        public override bool IsConnected {
            get { return !m_Dispose && m_Con.IsConnected; }
        }

        public override void Disconnect() {
            Dispose();
        }

        public override void OnUpdate() {
            if (m_Dispose) {
                return;
            }

            if (m_RecvPackets.TryDequeue(out Packet packet)) {
                try {
                    Invoke(packet);
                } catch (Exception e) {
                    Logger.Error(e.ToString());
                }
            }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, byte channel) {
            if (m_Dispose) {
                return;
            }
            m_SendPackets.Enqueue(new Packet(cmd, buffer, channel));
        }

        public override void Dispose() {
            lock (this) {
                if (m_Dispose) {
                    return;
                }
                m_Dispose = true;
            }

            base.Dispose();

            m_Con.SendDisconnect();
            m_Con.Dispose();

            m_SendPackets.Clear();
            m_RecvPackets.Clear();
        }

        #region internal method
        internal int RawSend(byte[] buffer, int offset, int length) {
            if (m_Dispose) {
                return -20;
            }
            return m_Con.RawSend(buffer, offset, length);
        }

        internal int Send(byte[] buffer, int offset, int length) {
            if (m_Dispose) {
                return -20;
            }
            return m_Con.Send(buffer, offset, length);
        }

        internal int Recv(byte[] buffer, int offset, int length) {
            if (m_Dispose) {
                return -20;
            }

            return m_Con.Recv(buffer, offset, length);
        }

        internal void Input(byte[] buffer, int offset, int length) {
            if (m_Dispose) {
                return;
            }

            m_Con.Input(buffer, offset, length);
        }

        internal void Flush() {
            if (m_Dispose) {
                return;
            }

            m_Con.Flush();
        }

        internal void OnUpdate(ServerDataProcessing process) {
            if (m_Dispose) {
                return;
            }
            process.SendPackets(this, m_SendPackets);
            m_Con.OnUpdate(TimeUtil.Get1970ToNowMilliseconds());
            m_Heartbeat.OnUpdate(this);
        }

        internal void RecvReliablePackets(ServerDataProcessing process, List<Packet> packets, IKcpConnect connect) {
            if (m_Dispose) {
                return;
            }

            if (process.RecvReliablePackets(this, packets, connect, m_Heartbeat)) {
                HandlePackets(packets);
            }
        }

        internal void RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, ServerDataProcessing process, List<Packet> packets) {
            if (m_Dispose) {
                return;
            }

            if (process.RecvUnreliablePackets(rawBuffer, offset, count, packets)) {
                HandlePackets(packets);
            }
        }

        internal void SetConnectedStatus(bool status) {
            if (m_Dispose) {
                return;
            }

            m_Con.IsConnected = status;
        }

        internal uint Conv {
            get { return m_Conv; }
        }

        internal bool IsDispose {
            get { return m_Dispose; }
        }
        #endregion

        private void HandlePackets(List<Packet> packets) {
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
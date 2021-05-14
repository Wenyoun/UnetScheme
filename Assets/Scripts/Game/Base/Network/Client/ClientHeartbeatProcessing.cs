namespace Nice.Game.Base {
    internal class ClientHeartbeatProcessing {
        private long m_RecvMills;
        private long m_SendMills;
        private readonly byte[] m_RawBuffer;

        public ClientHeartbeatProcessing() {
            long current = TimeUtil.Get1970ToNowMilliseconds();
            m_RecvMills = current;
            m_SendMills = current - 1000000;
            m_RawBuffer = new byte[32];
        }

        public void UpdateHeartbeat() {
            m_RecvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void OnUpdate(ClientTransport transport, KcpCon kcp, long time) {
            long current = time;

            if (current - m_RecvMills >= HeartbaetConstants.Timeout_Interval_Mills) {
                transport.Disconnect(true, true);
                return;
            }

            if (current - m_SendMills >= HeartbaetConstants.Send_Interval_Mills) {
                m_SendMills = current;
                ByteWriteMemory write = new ByteWriteMemory(m_RawBuffer);
                write.Write(KcpConstants.Flag_Heartbeat);
                write.Write(kcp.Conv);
                kcp.Send(m_RawBuffer, 0, 8);
            }
        }
    }
}
namespace Nice.Game.Base
{
    internal class HeartbaetConstants
    {
        public const int Send_Interval_Mills = 10000;
        public const int Timeout_Interval_Mills = 30000;
    }

    internal class ClientHeartbeatProcessing
    {
        private long m_RecvMills;
        private long m_SendMills;
        private readonly byte[] m_RawBuffer;

        public ClientHeartbeatProcessing()
        {
            long current = TimeUtil.Get1970ToNowMilliseconds();
            m_RecvMills = current;
            m_SendMills = current - 1000000;
            m_RawBuffer = new byte[32];
        }

        public void UpdateHeartbeat()
        {
            m_RecvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void OnUpdate(ClientSocket socket, KcpCon kcp, long time)
        {
            long current = time;

            if (current - m_RecvMills >= HeartbaetConstants.Timeout_Interval_Mills)
            {
                socket.Disconnect();
                return;
            }

            if (current - m_SendMills >= HeartbaetConstants.Send_Interval_Mills)
            {
                m_SendMills = current;
                ByteWriteMemory write = new ByteWriteMemory(m_RawBuffer);
                write.Write(KcpConstants.Flag_Heartbeat);
                write.Write(kcp.Conv);
                kcp.Send(m_RawBuffer, 0, 8);
            }
        }
    }

    internal class ServerHeartbeatProcessing
    {
        private long m_RecvMills;

        public ServerHeartbeatProcessing()
        {
            m_RecvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void UpdateHeartbeat(ServerChannel channel, byte[] rawBuffer, int offset, int length)
        {
            m_RecvMills = TimeUtil.Get1970ToNowMilliseconds();
            channel.Send(rawBuffer, offset, length);
        }

        public void OnUpdate(ServerChannel channel)
        {
            long current = TimeUtil.Get1970ToNowMilliseconds();
            if (current - m_RecvMills > HeartbaetConstants.Timeout_Interval_Mills)
            {
                channel.Disconnect();
            }
        }
    }
}
namespace Nice.Game.Base {
    internal class ServerHeartbeatProcessing {
        private long m_RecvMills;

        public ServerHeartbeatProcessing() {
            m_RecvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void UpdateHeartbeat(ServerChannel channel, byte[] rawBuffer, int offset, int length) {
            m_RecvMills = TimeUtil.Get1970ToNowMilliseconds();
            channel.Send(rawBuffer, offset, length);
        }

        public void OnUpdate(ServerChannel channel) {
            long current = TimeUtil.Get1970ToNowMilliseconds();
            if (current - m_RecvMills > HeartbaetConstants.Timeout_Interval_Mills) {
                channel.Disconnect();
            }
        }
    }
}
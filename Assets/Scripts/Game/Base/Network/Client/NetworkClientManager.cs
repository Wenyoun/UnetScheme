namespace Nice.Game.Base {
    public class NetworkClientManager {
        private static NetworkClient m_Network;

        public static void Start(string host, int port, IClientHandler handler) {
            if (m_Network != null) {
                return;
            }
            m_Network = new NetworkClient(handler);
            m_Network.Connect(host, port);
        }

        public static void Disconnect() {
            if (m_Network == null) {
                return;
            }
            m_Network.Disconnect();
            m_Network = null;
        }

        public static void Send(ushort cmd, ByteBuffer buffer, byte channel) {
            if (m_Network != null) {
                m_Network.Send(cmd, buffer, channel);
            }
        }
    }
}
using Nice.Game.Base;

namespace Nice.Game.Client {
    public class NetworkClientManager {
        private static bool m_Dispose;
        private static ClientChannel m_Channel;
        private static Connection m_Connection;

        public static void Init() {
            if (m_Dispose) {
                return;
            }

            m_Dispose = false;
            m_Channel = new ClientChannel();
            SystemLoop.AddUpdate(OnUpdate);
        }

        public static void Dispose() {
            if (m_Dispose) {
                return;
            }

            m_Dispose = false;
            SystemLoop.RemoveUpdate(OnUpdate);

            if (m_Channel != null) {
                m_Channel.Dispose();
                m_Channel = null;
            }

            if (m_Connection != null) {
                m_Connection.Dispose();
                m_Connection = null;
            }
        }

        public static void Connect(string host, int port) {
            if (m_Dispose) {
                return;
            }

            m_Channel.SetConnect(new ClientConnect());
            m_Channel.Connect(host, port);
        }

        public static void Disconnect() {
            if (m_Dispose) {
                return;
            }

            m_Channel.Disconnect();
        }

        public static void Send(ushort cmd, ByteBuffer buffer) {
            if (m_Dispose) {
                return;
            }
            m_Channel.Send(cmd, buffer, MsgChannel.Unreliable);
        }

        public static Connection Connection {
            get { return m_Connection; }
        }

        private static void OnUpdate() {
            if (m_Dispose) {
                return;
            }
            m_Channel.OnUpdate();
        }

        private static void AddChannel(IChannel channel) {
            if (m_Dispose) {
                return;
            }

            m_Connection = new Connection(channel);
            RegisterProtocols(m_Connection);
            Client.Ins.World.DispatchMessage(MessageConstants.Connect_Success);
        }

        private static void RemoveChannel(IChannel channel) {
            if (m_Dispose) {
                return;
            }

            m_Connection.Dispose();
            Client.Ins.World.DispatchMessage(MessageConstants.Connect_Error);
        }

        private static void RegisterProtocols(Connection connection) {
            if (m_Dispose) {
                return;
            }

            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
        }

        private class ClientConnect : IClientConnect {
            public void OnTimeout(IChannel channel) {
            }

            public void OnError(IChannel channel) {
            }

            public void OnConnect(IChannel channel) {
                AddChannel(channel);
            }

            public void OnDisconnect(IChannel channel) {
                RemoveChannel(channel);
            }
        }
    }
}
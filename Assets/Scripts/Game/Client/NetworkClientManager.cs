using Nice.Game.Base;

namespace Nice.Game.Client
{
    public static class NetworkClientManager
    {
        private static bool m_Initialize;
        private static ClientChannel m_Channel;
        private static Connection m_Connection;

        public static void Init()
        {
            if (m_Initialize)
            {
                return;
            }

            m_Initialize = true;
            m_Channel = new ClientChannel();
        }

        public static void Dispose()
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Initialize = false;

            if (m_Channel != null)
            {
                m_Channel.Dispose();
                m_Channel = null;
            }

            if (m_Connection != null)
            {
                m_Connection.Dispose();
                m_Connection = null;
            }
        }

        public static void Connect(string host, int port)
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Channel.Connect(host, port, new ClientCallback());
        }

        public static void Disconnect()
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Channel.Disconnect();
        }

        public static void OnUpdate()
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Channel.Dispatcher();
        }

        public static void Send(ushort cmd, ByteBuffer buffer)
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Channel.Send(cmd, buffer);
        }

        public static Connection Connection
        {
            get { return m_Connection; }
        }

        private static void AddChannel(IChannel channel)
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Connection = new Connection(channel);
            RegisterProtocols(m_Connection);
            Client.Ins.World.Dispatcher(MessageConstants.Connect_Success);
        }

        private static void RemoveChannel(IChannel channel)
        {
            if (!m_Initialize)
            {
                return;
            }

            m_Connection.Dispose();
            Client.Ins.World.Dispatcher(MessageConstants.Connect_Error);
        }

        private static void RegisterProtocols(Connection connection)
        {
            if (!m_Initialize)
            {
                return;
            }

            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
        }

        private class ClientCallback : IClientCallback
        {
            public void OnServerConnect(IChannel channel)
            {
                NetworkClientManager.AddChannel(channel);
            }

            public void OnServerDisconnect(IChannel channel)
            {
                NetworkClientManager.RemoveChannel(channel);
            }
        }
    }
}
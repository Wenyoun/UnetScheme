namespace Nice.Game.Base
{
    internal class NetworkClient : IClientListener
    {
        private Connection m_Connection;
        private IConnectionHandler m_Handler;

        public void Connect(string host, int port, IConnectionHandler handler)
        {
            if (m_Connection != null)
            {
                return;
            }

            SystemLoop.AddUpdate(OnUpdate);
            m_Handler = handler;
            ClientChannel channel = new ClientChannel();
            channel.Connect(host, port, this);
            m_Connection = new Connection(channel);
        }

        public void Disconnect()
        {
            SystemLoop.RemoveUpdate(OnUpdate);
            if (m_Connection != null)
            {
                m_Connection.Disconnect();
                m_Connection = null;
            }
        }

        public void Send(ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (m_Connection == null)
            {
                return;
            }

            m_Connection.Send(cmd, buffer, channel);
        }

        private void OnUpdate()
        {
            if (m_Connection == null)
            {
                return;
            }
            m_Connection.OnUpdate();
        }

        private void AddChannel(IChannel channel)
        {
            if (m_Connection == null)
            {
                return;
            }

            if (m_Handler != null)
            {
                m_Handler.OnAddConnection(m_Connection);
            }
        }

        private void RemoveChannel(IChannel channel)
        {
            if (m_Connection == null)
            {
                return;
            }

            if (m_Handler != null)
            {
                m_Handler.OnRemoveConnection(m_Connection);
            }
        }

        public void OnConnect(IChannel channel)
        {
            AddChannel(channel);
        }

        public void OnTimeout(IChannel channel)
        {
            RemoveChannel(channel);
        }

        public void OnError(IChannel channel)
        {
            RemoveChannel(channel);
        }

        public void OnDisconnect(IChannel channel)
        {
            RemoveChannel(channel);
        }
    }
}
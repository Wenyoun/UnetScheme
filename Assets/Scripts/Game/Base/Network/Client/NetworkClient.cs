namespace Nice.Game.Base
{
    internal class NetworkClient : IClientListener
    {
        private Connection m_Connection;
        private IConnectionHandle m_Handle;

        public void Connect(string host, int port, IConnectionHandle handle)
        {
            if (m_Connection != null)
            {
                return;
            }

            SystemLoop.AddUpdate(OnUpdate);
            m_Handle = handle;
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

            if (m_Handle != null)
            {
                m_Handle.OnAddConnection(m_Connection);
            }
        }

        private void RemoveChannel(IChannel channel)
        {
            if (m_Connection == null)
            {
                return;
            }

            if (m_Handle != null)
            {
                m_Handle.OnRemoveConnection(m_Connection);
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
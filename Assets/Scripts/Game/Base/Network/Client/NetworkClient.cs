namespace Nice.Game.Base {
    internal class NetworkClient : IClientConnect {
        private ClientChannel m_Channel;
        private Connection m_Connection;
        private IClientHandler m_Handler;

        public NetworkClient(IClientHandler handler) {
            m_Handler = handler;
        }

        public void Connect(string host, int port) {
            if (m_Channel != null) {
                return;
            }

            SystemLoop.AddUpdate(OnUpdate);
            m_Channel = new ClientChannel();
            m_Channel.Connect(host, port, this);
        }

        public void Disconnect() {
            SystemLoop.RemoveUpdate(OnUpdate);
            if (m_Connection != null) {
                m_Connection.Disconnect();
                m_Connection = null;
            }
            m_Channel = null;
        }

        public void Send(ushort cmd, ByteBuffer buffer, ChannelType channel) {
            if (m_Connection == null) {
                return;
            }
            m_Connection.Send(cmd, buffer, channel);
        }

        private void OnUpdate() {
            if (m_Channel == null) {
                return;
            }
            m_Channel.OnUpdate();
        }

        private void AddChannel(IChannel channel) {
            if (m_Connection != null) {
                return;
            }

            m_Connection = new Connection(channel);
            if (m_Handler != null) {
                m_Handler.OnAddConnection(m_Connection);
            }
        }

        private void RemoveChannel(IChannel channel) {
            if (m_Connection == null) {
                return;
            }

            if (m_Handler != null) {
                m_Handler.OnRemoveConnection(m_Connection);
            }
        }

        public void OnConnect(IChannel channel) {
            AddChannel(channel);
        }

        public void OnTimeout(IChannel channel) {
            RemoveChannel(channel);
        }

        public void OnError(IChannel channel) {
            RemoveChannel(channel);
        }

        public void OnDisconnect(IChannel channel) {
            RemoveChannel(channel);
        }
    }
}
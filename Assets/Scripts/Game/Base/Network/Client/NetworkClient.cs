namespace Nice.Game.Base {
    internal class NetworkClient : IClientConnect {
        private Connection m_Connection;
        private ClientChannel m_Channel;
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
            m_Channel = null;
            if (m_Connection != null) {
                m_Connection.Dispose();
                m_Connection = null;
            }
        }

        public void Send(ushort cmd, ByteBuffer buffer, byte channel) {
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
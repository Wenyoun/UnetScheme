using System;

namespace Nice.Game.Base {
    public class ClientChannel : AbsChannel {
        private bool m_Dispose;
        private ClientSocket m_Socket;
        private IClientConnect m_Connect;

        public override uint ChannelId {
            get { return !m_Dispose ? m_Socket.ConId : 0; }
        }

        public override bool IsConnected {
            get { return !m_Dispose && m_Socket.IsConnected; }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, byte channel) {
            if (m_Dispose || !m_Socket.IsConnected) {
                return;
            }
            m_Socket.Send(new Packet(cmd, buffer, channel));
        }

        public override void Dispose() {
            if (m_Dispose) {
                return;
            }
            ClearHandlers();
            m_Socket.Dispose();
            m_Dispose = true;
            m_Socket = null;
            m_Connect = null;
        }

        public override void Disconnect() {
            if (m_Dispose) {
                return;
            }
            m_Socket.Disconnect();
        }

        public override void OnUpdate() {
            if (m_Dispose) {
                return;
            }

            m_Socket.OnUpdate();
            while (m_Socket.Recv(out Packet packet)) {
                try {
                    Invoke(packet);
                } catch (Exception e) {
                    Logger.Error(e.ToString());
                }
            }
        }

        public void SetConnect(IClientConnect connect) {
            if (m_Dispose) {
                return;
            }
            m_Connect = connect;
        }

        public void Connect(string host, int port) {
            if (m_Dispose) {
                return;
            }
            m_Socket = new ClientSocket();
            m_Socket.Connect(host, port);
            RegisterMessages();
        }

        private void RegisterMessages() {
            m_Socket.Register(ClientSocket.Msg_Timeout, OnTimeout);
            m_Socket.Register(ClientSocket.Msg_Error, OnError);
            m_Socket.Register(ClientSocket.Msg_Success, OnSuccess);
            m_Socket.Register(ClientSocket.Msg_Disconnect, OnDisconnect);
        }

        private void OnTimeout() {
            if (m_Connect != null) {
                m_Connect.OnTimeout(this);
            }
        }

        private void OnError() {
            if (m_Connect != null) {
                m_Connect.OnError(this);
            }
        }

        private void OnSuccess() {
            if (m_Connect != null) {
                m_Connect.OnConnect(this);
            }
        }

        private void OnDisconnect() {
            if (m_Connect != null) {
                m_Connect.OnDisconnect(this);
            }
        }
    }
}
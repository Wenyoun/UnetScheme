using System;

namespace Nice.Game.Base {
    public class ClientChannel : AbsChannel {
        private bool m_Dispose;
        private IClientConnect m_Connect;
        private ClientTransport m_Transport;

        public override uint ChannelId {
            get { return !m_Dispose ? m_Transport.ConId : 0; }
        }

        public override bool IsConnected {
            get { return !m_Dispose && m_Transport.IsConnected; }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, byte channel) {
            if (m_Dispose || !m_Transport.IsConnected) {
                return;
            }
            m_Transport.Send(new Packet(cmd, buffer, channel));
        }

        public override void Dispose() {
            if (m_Dispose) {
                return;
            }
            m_Dispose = true;
            m_Transport.Dispose();
            base.Dispose();
        }

        public override void Disconnect() {
            if (m_Dispose) {
                return;
            }
            m_Transport.Disconnect(false, false);
        }

        public override void OnUpdate() {
            if (m_Dispose) {
                return;
            }

            while (m_Transport.Recv(out Packet packet)) {
                try {
                    Invoke(packet);
                } catch (Exception e) {
                    Logger.Error(e.ToString());
                }
            }
            
            m_Transport.OnUpdate();
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
            m_Transport = new ClientTransport();
            m_Transport.Connect(host, port);
            RegisterMessages();
        }

        private void RegisterMessages() {
            m_Transport.Register(Msg.Timeout, OnTimeout);
            m_Transport.Register(Msg.Error, OnError);
            m_Transport.Register(Msg.Success, OnSuccess);
            m_Transport.Register(Msg.Disconnect, OnDisconnect);
        }

        private void OnTimeout() {
            if (m_Connect != null) {
                Logger.Debug("OnTimeout");
                m_Connect.OnTimeout(this);
            }
        }

        private void OnError() {
            if (m_Connect != null) {
                Logger.Debug("OnError");
                Dispose();
                m_Connect.OnError(this);
            }
        }

        private void OnSuccess() {
            if (m_Connect != null) {
                Logger.Debug("OnSuccess");
                m_Connect.OnConnect(this);
            }
        }

        private void OnDisconnect() {
            if (m_Connect != null) {
                Logger.Debug("OnDisconnect");
                Dispose();
                m_Connect.OnDisconnect(this);
            }
        }
    }
}
using System;

namespace Nice.Game.Base
{
    internal class ClientChannel : AbsChannel
    {
        private bool m_Dispose;
        private IClientListener m_Listener;
        private ClientTransport m_Transport;

        public override uint ChannelId
        {
            get { return !m_Dispose ? m_Transport.ConId : 0; }
        }

        public override bool IsConnected
        {
            get { return !m_Dispose && m_Transport.IsConnected; }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (m_Dispose || !m_Transport.IsConnected)
            {
                return;
            }
            m_Transport.Send(new Packet(cmd, buffer, channel));
        }

        public override void Dispose()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Dispose = true;
            m_Transport.Dispose();
            ClearMsgHandlers();
            m_Listener = null;
            m_Transport = null;
        }

        public override void Disconnect()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Transport.Disconnect(true, false);
        }

        public override void OnUpdate()
        {
            if (m_Dispose)
            {
                return;
            }

            while (m_Transport.Recv(out Packet p))
            {
                try
                {
                    CallMsgHandler(p.Cmd, p.Buffer);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }

            m_Transport.OnUpdate();
        }

        internal void Connect(string host, int port, IClientListener listener)
        {
            if (m_Dispose)
            {
                throw new ObjectDisposedException("ClientChannel already disposed!");
            }
            m_Listener = listener;
            m_Transport = new ClientTransport();
            m_Transport.Connect(host, port);
            RegisterMessages();
        }

        private void RegisterMessages()
        {
            m_Transport.Register(Msg.Timeout, OnTimeout);
            m_Transport.Register(Msg.Error, OnError);
            m_Transport.Register(Msg.Success, OnSuccess);
            m_Transport.Register(Msg.Disconnect, OnDisconnect);
        }

        private void OnTimeout()
        {
            if (m_Listener != null)
            {
                Logger.Debug("OnTimeout");
                m_Listener.OnTimeout(this);
            }
        }

        private void OnError()
        {
            if (m_Listener != null)
            {
                Logger.Debug("OnError");
                Dispose();
                m_Listener.OnError(this);
            }
        }

        private void OnSuccess()
        {
            if (m_Listener != null)
            {
                Logger.Debug("OnSuccess");
                m_Listener.OnConnect(this);
            }
        }

        private void OnDisconnect()
        {
            if (m_Listener != null)
            {
                Logger.Debug("OnDisconnect");
                Dispose();
                m_Listener.OnDisconnect(this);
            }
        }
    }
}
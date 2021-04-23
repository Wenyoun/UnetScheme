using System;
using UnityEngine;

namespace Nice.Game.Base
{
    public class ClientChannel : AbsChannel
    {
        private bool m_Dispose;
        private IClientCallback m_Callback;
        private ClientSocket m_SocketSocket;

        public override uint ChannelId
        {
            get { return !m_Dispose ? m_SocketSocket.ConId : 0; }
        }

        public override bool IsConnected
        {
            get { return !m_Dispose && m_SocketSocket.IsConnected; }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, byte channel)
        {
            if (m_Dispose)
            {
                return;
            }
            m_SocketSocket.Send(new Packet(cmd, buffer, channel));
        }

        public override void Dispose()
        {
            if (m_Dispose)
            {
                return;
            }

            Disconnect();
            m_Dispose = true;
            base.Dispose();
        }

        public override void Disconnect()
        {
            ClearHandlers();
            if (m_SocketSocket != null)
            {
                m_SocketSocket.Dispose();
                m_SocketSocket = null;
            }
        }

        public override void Dispatcher()
        {
            if (m_Dispose)
            {
                return;
            }
            HandlePackets();
        }

        public void Connect(string host, int port, IClientCallback callback)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Callback = callback;
            m_SocketSocket = new ClientSocket();
            m_SocketSocket.Connect(host, port);
            RegisterMessages();
        }

        private void HandlePackets()
        {
            if (m_Dispose)
            {
                return;
            }

            try
            {
                while (m_SocketSocket.Recv(out Packet packet))
                {
                    Call(packet);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void RegisterMessages()
        {
            Register(ClientSocket.Success, OnConnectSuccess);
            Register(ClientSocket.Error, OnConnectError);
        }

        private void OnConnectSuccess(ChannelMessage msg)
        {
            m_Callback.OnServerConnect(this);
        }

        private void OnConnectError(ChannelMessage msg)
        {
            m_Callback.OnServerDisconnect(this);
        }
    }
}
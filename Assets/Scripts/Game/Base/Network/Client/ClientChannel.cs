using System;
using UnityEngine;

namespace Nice.Game.Base
{
    public class ClientChannel : AbsChannel
    {
        private byte m_Status;
        private IClientCallback m_Callback;
        private KcpUdpClient m_KcpUdpClient;

        public ClientChannel()
        {
            m_Status = KcpUdpClient.None;
        }

        public override long ChannelId
        {
            get { return m_KcpUdpClient != null ? m_KcpUdpClient.ConId : -1; }
        }

        public override bool IsConnected
        {
            get { return m_KcpUdpClient != null && m_KcpUdpClient.IsConnected; }
        }

        public override void Send(ushort cmd, ByteBuffer buffer, byte channel)
        {
            if (m_KcpUdpClient != null)
            {
                m_KcpUdpClient.Send(new Packet(cmd, buffer, channel));
            }
        }

        public override void Dispose()
        {
            Disconnect();
            base.Dispose();
        }

        public override void Disconnect()
        {
            ClearHandlers();
            m_Status = KcpUdpClient.None;
            if (m_KcpUdpClient != null)
            {
                m_KcpUdpClient.Dispose();
                m_KcpUdpClient = null;
            }
        }

        public override void Dispatcher()
        {
            if (m_KcpUdpClient == null)
            {
                return;
            }

            HandlePackets();
            CheckConnectStatus();
        }

        public void Connect(string host, int port, IClientCallback callback)
        {
            if (m_Status == KcpUdpClient.Connecting)
            {
                return;
            }

            m_Callback = callback;
            m_Status = KcpUdpClient.Connecting;
            m_KcpUdpClient = new KcpUdpClient();
            m_KcpUdpClient.Connect(host, port);
        }

        private void HandlePackets()
        {
            if (m_KcpUdpClient == null)
            {
                return;
            }

            try
            {
                while (m_KcpUdpClient.Recv(out Packet packet))
                {
                    Call(packet);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void CheckConnectStatus()
        {
            if (m_Status == KcpUdpClient.None)
            {
                return;
            }

            if (m_Status == KcpUdpClient.Connecting)
            {
                if (m_KcpUdpClient.Status == KcpUdpClient.Success)
                {
                    m_Status = KcpUdpClient.Success;
                    m_Callback.OnServerConnect(this);
                }
                else if (m_KcpUdpClient.Status == KcpUdpClient.Error)
                {
                    m_Status = KcpUdpClient.None;
                    m_Callback.OnServerDisconnect(this);
                    Disconnect();
                }
            }
            else if (m_Status == KcpUdpClient.Error)
            {
                m_Status = KcpUdpClient.None;
                m_Callback.OnServerDisconnect(this);
                Disconnect();
            }
            else if (m_Status == KcpUdpClient.Success)
            {
                if (!m_KcpUdpClient.IsConnected)
                {
                    m_Status = KcpUdpClient.None;
                    m_Callback.OnServerDisconnect(this);
                    Disconnect();
                }
            }
        }
    }
}
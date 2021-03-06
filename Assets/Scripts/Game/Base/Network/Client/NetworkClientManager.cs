﻿namespace Nice.Game.Base
{
    public static class NetworkClientManager
    {
        private static NetworkClient m_Network;

        public static void Connect(string host, int port, IConnectionHandler handler)
        {
            if (m_Network != null)
            {
                return;
            }
            m_Network = new NetworkClient();
            m_Network.Connect(host, port, handler);
        }

        public static void Disconnect()
        {
            if (m_Network == null)
            {
                return;
            }
            m_Network.Disconnect();
            m_Network = null;
        }

        public static void Send(ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (m_Network != null)
            {
                m_Network.Send(cmd, buffer, channel);
            }
        }
    }
}
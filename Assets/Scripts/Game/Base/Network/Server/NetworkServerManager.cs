namespace Nice.Game.Base
{
    public class NetworkServerManager
    {
        private static NetworkServer m_Network;

        public static void Bind(int port, IConnectionHandle handle)
        {
            if (m_Network != null)
            {
                return;
            }
            SystemLoop.AddUpdate(OnUpdate);
            m_Network = new NetworkServer();
            m_Network.Bind(port, handle);
        }

        public static void Dispose()
        {
            if (m_Network == null)
            {
                return;
            }
            SystemLoop.RemoveUpdate(OnUpdate);
            m_Network.Dispose();
            m_Network = null;
        }

        public static void Send(IConnection connection, ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (m_Network != null)
            {
                m_Network.Send(connection.ConnectionId, cmd, buffer, channel);
            }
        }

        private static void OnUpdate()
        {
            if (m_Network != null)
            {
                m_Network.OnUpdate();
            }
        }
    }
}
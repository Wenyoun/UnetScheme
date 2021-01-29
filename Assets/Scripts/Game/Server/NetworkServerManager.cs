using Nice.Game.Base;
using System.Collections.Generic;

namespace Nice.Game.Server
{
    public static class NetworkServerManager
    {
        private static bool m_Initalize;
        private static NetworkServer m_Network;
        private static Dictionary<long, Connection> m_Connections;

        public static void Init()
        {
            if (m_Initalize)
            {
                return;
            }
            m_Initalize = true;
            m_Network = new NetworkServer();
            m_Connections = new Dictionary<long, Connection>();
        }

        public static void Dispose()
        {
            if (!m_Initalize)
            {
                return;
            }
            m_Initalize = false;
            m_Network.Dispose();
            m_Connections.Clear();
        }

        public static void Bind(int port)
        {
            if (!m_Initalize)
            {
                return;
            }
            m_Network.Bind(port, new ServerCallback());
        }

        public static void OnUpdate()
        {
            if (!m_Initalize)
            {
                return;
            }
            m_Network.OnUpdate();
        }

        public static void Send(Connection connection, ushort cmd, ByteBuffer buffer)
        {
            if (!m_Initalize || connection == null || buffer == null)
            {
                return;
            }

            long connectionId = connection.ConnectionId;
            if (m_Connections.ContainsKey(connectionId))
            {
                connection.Send(cmd, buffer);
            }
        }

        public static void Broadcast(ushort cmd, ByteBuffer buffer)
        {
            if (!m_Initalize)
            {
                return;
            }

            using (Dictionary<long, Connection>.Enumerator its = m_Connections.GetEnumerator())
            {
                while (its.MoveNext())
                {
                    its.Current.Value.Send(cmd, buffer);
                }
            }
        }

        private static void AddChannel(IChannel channel)
        {
            long connectionId = channel.ChannelId;
            if (!m_Connections.ContainsKey(connectionId))
            {
                Connection connection = new Connection(channel);
                m_Connections.Add(connectionId, connection);
                RegisterProtocols(connection);
            }
        }

        private static void RemoveChannel(IChannel channel)
        {
            Connection connection;
            long connectionId = channel.ChannelId;
            if (m_Connections.TryGetValue(connectionId, out connection))
            {
                m_Connections.Remove(connectionId);
                connection.Dispose();
            }
        }

        private static void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        private class ServerCallback : IServerCallback
        {
            public void OnClientConnect(IChannel channel)
            {
                NetworkServerManager.AddChannel(channel);
            }

            public void OnClientDisconnect(IChannel channel)
            {
                NetworkServerManager.RemoveChannel(channel);
            }
        }
    }
}
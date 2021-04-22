using Nice.Game.Base;
using System.Collections.Generic;

namespace Nice.Game.Server
{
    public class NetworkServerManager
    {
        private static bool m_Dispose;
        private static NetworkServer m_Network;
        private static Dictionary<uint, Connection> m_Connections;

        public static void Init()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Dispose = false;
            m_Network = new NetworkServer();
            m_Connections = new Dictionary<uint, Connection>();
            SystemLoop.AddUpdate(OnUpdate);
        }

        public static void Dispose()
        {
            if (m_Dispose)
            {
                return;
            }

            m_Dispose = false;
            SystemLoop.RemoveUpdate(OnUpdate);

            if (m_Network != null)
            {
                m_Network.Dispose();
                m_Network = null;
            }

            if (m_Connections != null)
            {
                m_Connections.Clear();
                m_Connections = null;
            }
        }

        public static void Bind(int port)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Network.Bind(port, new ServerCallback());
        }

        public static void Send(Connection connection, ushort cmd, ByteBuffer buffer)
        {
            if (m_Dispose)
            {
                return;
            }

            uint connectionId = connection.ConnectionId;
            if (m_Connections.ContainsKey(connectionId))
            {
                connection.Send(cmd, buffer);
            }
        }

        public static void Broadcast(ushort cmd, ByteBuffer buffer)
        {
            if (m_Dispose)
            {
                return;
            }

            using (Dictionary<uint, Connection>.Enumerator its = m_Connections.GetEnumerator())
            {
                while (its.MoveNext())
                {
                    its.Current.Value.Send(cmd, buffer);
                }
            }
        }

        private static void OnUpdate()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Network.OnUpdate();
        }

        private static void AddChannel(IChannel channel)
        {
            uint connectionId = channel.ChannelId;
            if (!m_Connections.ContainsKey(connectionId))
            {
                Connection connection = new Connection(channel);
                m_Connections.Add(connectionId, connection);
                RegisterProtocols(connection);
            }
        }

        private static void RemoveChannel(IChannel channel)
        {
            uint connectionId = channel.ChannelId;
            if (m_Connections.TryGetValue(connectionId, out Connection connection))
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
                AddChannel(channel);
            }

            public void OnClientDisconnect(IChannel channel)
            {
                RemoveChannel(channel);
            }
        }
    }
}
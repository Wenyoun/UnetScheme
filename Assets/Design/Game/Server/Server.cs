using UnityEngine.Networking;

using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class Server
    {
        public static Server Ins = new Server();

        private Dictionary<int, Connection> m_Connections;

        private Server()
        {
            m_Connections = new Dictionary<int, Connection>();
        }

        public void Init()
        {
            m_Connections.Clear();
        }

        public void Dispose()
        {
            foreach (Connection connection in m_Connections.Values)
            {
                connection.Dispose();
            }
            m_Connections.Clear();
        }

        public void Broadcast(Connection target, NetworkWriter writer)
        {
            foreach (Connection connection in m_Connections.Values)
            {
                if (target != connection)
                {
                    connection.Send(writer);
                }
            }
        }

        public void OnStartServer()
        {
        }

        public void OnStopServer()
        {
        }

        public void OnClientConnect(NetworkConnection net)
        {
            AddConnection(net);
        }

        public void OnClientDisconnect(NetworkConnection net)
        {
            RemoveConnection(net);
        }

        private void AddConnection(NetworkConnection net)
        {
            if (!m_Connections.ContainsKey(net.connectionId))
            {
                Connection connection = new Connection(net);
                RegisterProtocols(connection);
                m_Connections.Add(net.connectionId, connection);
            }
        }

        private void RemoveConnection(NetworkConnection net)
        {
            Connection connection = null;
            if (m_Connections.TryGetValue(net.connectionId, out connection))
            {
                connection.Dispose();
                m_Connections.Remove(net.connectionId);
            }
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<ServerProtocolHandler>();
        }
    }
}
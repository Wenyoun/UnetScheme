using UnityEngine.Networking;
using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class Server : AbsMachine
    {
        public static Server Ins = new Server();
        private SyncAttributeMgr m_SyncAttributeMgr;
        private Dictionary<int, Connection> m_Connections;

        private Server()
        {
        }

        public override void OnInit()
        {
            base.OnInit();
            m_SyncAttributeMgr = new SyncAttributeMgr();
            m_Connections = new Dictionary<int, Connection>();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            ClearConnections();
            m_SyncAttributeMgr = null;
        }

        public void Send(Connection target, NetworkWriter writer)
        {
            if (target != null)
            {
                target.Send(writer);
            }
        }

        public void Broadcast(Connection target, NetworkWriter writer)
        {
            if (m_Connections != null)
            {
                foreach (Connection connection in m_Connections.Values)
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

        public override void OnNetConnect(NetworkConnection network)
        {
            int netId = network.connectionId;
            if (!m_Connections.ContainsKey(netId))
            {
                Connection connection = new Connection();
                connection.OnConnect(network);
                m_Connections.Add(netId, connection);
                RegisterProtocols(connection);
            }
        }

        public override void OnNetDisconnect(NetworkConnection network)
        {
            int netId = network.connectionId;
            Connection connection = null;
            if (m_Connections.TryGetValue(netId, out connection))
            {
                m_Connections.Remove(netId);
                connection.Dispose();
            }
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            m_SyncAttributeMgr.OnUpdate(delta);
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        private void ClearConnections()
        {
            foreach (Connection connection in m_Connections.Values)
            {
                connection.Dispose();
            }
            m_Connections.Clear();
            m_Connections = null;
        }
    }
}
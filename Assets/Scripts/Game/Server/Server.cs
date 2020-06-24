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
            m_Connections = new Dictionary<int, Connection>();
        }

        public override void OnInit()
        {
            base.OnInit();
            m_SyncAttributeMgr = new SyncAttributeMgr();
            m_Connections.Clear();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            m_SyncAttributeMgr = null;
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
                connection.Send(writer);
            }
        }

        public void OnStartServer()
        {
        }

        public void OnStopServer()
        {
        }

        public override void OnNetConnect(NetworkConnection net)
        {
            AddConnection(net);
        }

        public override void OnNetDisconnect(NetworkConnection net)
        {
            RemoveConnection(net);
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

        private void AddConnection(NetworkConnection net)
        {
            if (!m_Connections.ContainsKey(net.connectionId))
            {
                Connection connection = new Connection(net);
                m_Connections.Add(net.connectionId, connection);
                RegisterProtocols(connection);
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
    }
}
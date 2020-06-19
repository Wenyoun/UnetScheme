using UnityEngine.Networking;
using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class Server : AbsMachine
    {
        public static Server Ins = new Server();
        private SyncAttributeMgr m_SyncAttributeMgr;
        private Dictionary<int, Connection> Connections;

        private Server()
        {
            Connections = new Dictionary<int, Connection>();
        }

        public override void OnInit()
        {
            base.OnInit();
            m_SyncAttributeMgr = new SyncAttributeMgr();
            Connections.Clear();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            m_SyncAttributeMgr = null;
            foreach (Connection connection in Connections.Values)
            {
                connection.Dispose();
            }
            Connections.Clear();
        }

        public void Broadcast(Connection target, NetworkWriter writer)
        {
            foreach (Connection connection in Connections.Values)
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

        public void OnClientConnect(NetworkConnection net)
        {
            AddConnection(net);
        }

        public void OnClientDisconnect(NetworkConnection net)
        {
            RemoveConnection(net);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            m_SyncAttributeMgr.OnUpdate(delta);
        }

        public override void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        private void AddConnection(NetworkConnection net)
        {
            if (!Connections.ContainsKey(net.connectionId))
            {
                Connection connection = new Connection(net);
                Connections.Add(net.connectionId, connection);
                RegisterProtocols(connection);
            }
        }

        private void RemoveConnection(NetworkConnection net)
        {
            Connection connection = null;
            if (Connections.TryGetValue(net.connectionId, out connection))
            {
                Connections.Remove(net.connectionId);
                connection.Dispose();
            }
        }
    }
}
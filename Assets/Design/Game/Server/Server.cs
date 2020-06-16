using UnityEngine.Networking;

using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class Server : IUpdate, IFixedUpdate
    {
        public static Server Ins = new Server();
        public EntityMgr EntityMgr { get; private set; }
        public UpdateMgr UpdateMgr { get; private set; }
        public MessageMgr MessageMgr { get; private set; }
        public TimerMgr TimerMgr { get; private set; }
        private SyncAttributeMgr m_SyncAttributeMgr;
        private Dictionary<int, Connection> m_Connections;

        private Server()
        {
            EntityMgr = new EntityMgr();
            UpdateMgr = new UpdateMgr();
            MessageMgr = new MessageMgr();
            TimerMgr = new TimerMgr();
            m_Connections = new Dictionary<int, Connection>();
        }

        public void Init()
        {
            UpdateMgr.Init();
            MessageMgr.Init();
            TimerMgr.Init();
            EntityMgr.Init();
            m_SyncAttributeMgr = new SyncAttributeMgr();
            m_Connections.Clear();
        }

        public void Dispose()
        {
            EntityMgr.Dispose();
            TimerMgr.Dispose();
            MessageMgr.Dispose();
            UpdateMgr.Dispose();
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

        public void OnClientConnect(NetworkConnection net)
        {
            AddConnection(net);
        }

        public void OnUpdate(float delta)
        {
            TimerMgr.OnUpdate(delta);
            UpdateMgr.OnUpdate(delta);
            EntityMgr.OnUpdate(delta);
            m_SyncAttributeMgr.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            UpdateMgr.OnFixedUpdate(delta);
            EntityMgr.OnFixedUpdate(delta);
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
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        private void OnLateUpdate()
        {
            
        }
    }
}
﻿using Zyq.Game.Base;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class Server : AbsMachine
    {
        public static Server Ins = new Server();

        #region Fields
        public ServerEntityMgr m_EntityMgr;
        private SyncAttributeMgr m_SyncAttributeMgr;
        private Dictionary<int, Connection> m_Connections;
        #endregion

        #region Properties
        public ServerEntityMgr EntityMgr => m_EntityMgr;
        #endregion

        private Server()
        {
        }

        public override void OnInit()
        {
            base.OnInit();
            m_EntityMgr = new ServerEntityMgr();
            m_SyncAttributeMgr = new SyncAttributeMgr();
            m_Connections = new Dictionary<int, Connection>();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            ClearConnections();
            m_EntityMgr.Dispose();
            m_EntityMgr = null;
            m_SyncAttributeMgr = null;
        }

        public void Send(Connection connection, NetworkWriter writer)
        {
            if (connection != null)
            {
                connection.Send(writer);
            }
        }

        public void Broadcast(NetworkWriter writer)
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
            if (m_EntityMgr != null)
            {
                m_EntityMgr.OnUpdate(delta);
            }
            if (m_SyncAttributeMgr != null)
            {
                m_SyncAttributeMgr.OnUpdate(delta);
            }
        }

        public override void OnFixedUpdate(float delta)
        {
            base.OnFixedUpdate(delta);
            if (m_EntityMgr != null)
            {
                m_EntityMgr.OnFixedUpdate(delta);
            }
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
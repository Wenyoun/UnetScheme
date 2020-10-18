using Zyq.Game.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Game.Server
{
    public class Server : AbsMachine, IServerCallback
    {
        public static Server Ins = new Server();

        #region Fields

        private ServerEntityMgr m_EntityMgr;
        private SyncAttributeMgr m_SyncAttributeMgr;
        private Dictionary<long, Connection> m_Connections;

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
            m_Connections = new Dictionary<long, Connection>();
            StartServer();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            ClearConnections();
            m_EntityMgr.Dispose();
            m_EntityMgr = null;
            m_SyncAttributeMgr = null;
            ServerNetworkMgr.Dispose();
        }

        public void StartServer()
        {
            ServerNetworkMgr.Bind(50000, this);
        }

        public void Send(Connection connection, ushort cmd, ByteBuffer buffer)
        {
            if (connection != null)
            {
                connection.Send(cmd, buffer);
            }
        }

        public void Broadcast(ushort cmd, ByteBuffer buffer)
        {
            if (m_Connections != null)
            {
                foreach (Connection connection in m_Connections.Values)
                {
                    connection.Send(cmd, buffer);
                }
            }
        }

        public void OnStartServer()
        {
        }

        public void OnStopServer()
        {
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            ServerNetworkMgr.OnUpdate();

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

        public void OnClientConnect(IChannel channel)
        {
            Debug.Log("Server OnClientConnect:" + channel.ChannelId);

            long netId = channel.ChannelId;
            if (!m_Connections.ContainsKey(netId))
            {
                Connection connection = new Connection();
                connection.OnConnect(channel);
                m_Connections.Add(netId, connection);
                RegisterProtocols(connection);
            }
        }

        public void OnClientDisconnect(IChannel channel)
        {
            Debug.Log("Server OnClientDisconnect:" + channel.ChannelId);

            long netId = channel.ChannelId;
            Connection connection = null;
            if (m_Connections.TryGetValue(netId, out connection))
            {
                m_Connections.Remove(netId);
                connection.Dispose();
            }
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        private void ClearConnections()
        {
            Dictionary<long, Connection>.Enumerator its = m_Connections.GetEnumerator();
            while (its.MoveNext())
            {
                its.Current.Value.Dispose();
            }

            m_Connections.Clear();
        }
    }
}
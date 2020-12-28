using Zyq.Game.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Game.Server
{
    public class World : AbsWorld, IServerCallback
    {
        private ServerNetwork m_Network;
        private Dictionary<long, Connection> m_Connections;

        public World() : base(1)
        {
            m_Network = new ServerNetwork(this);
            m_Connections = new Dictionary<long, Connection>();
        }

        protected override void Init()
        {
        }

        protected override void Clear()
        {
            ClearConnections();
            m_Network.Dispose();
            m_Connections.Clear();
        }

        public override void OnUpdate(float delta)
        {
            m_Network.OnUpdate();
            base.OnUpdate(delta);
            SyncAttributeMgr.OnUpdate(this, Entities.Entitys, delta);
        }

        public void Bind(int port)
        {
            m_Network.Bind(port);
        }

        public void CloseConnection(int connectionId)
        {
            if (m_Connections.TryGetValue(connectionId, out Connection connection))
            {
                m_Connections.Remove(connectionId);
                m_Network.CloseChannel(connectionId);
                connection.Dispose();
            }
        }

        public void Send(Connection connection, ushort cmd, ByteBuffer buffer)
        {
            if (m_Connections.TryGetValue(connection.ConnectionId, out connection))
            {
                connection.Send(cmd, buffer);
            }
        }

        public void Broadcast(ushort cmd, ByteBuffer buffer)
        {
            Dictionary<long, Connection>.Enumerator its = m_Connections.GetEnumerator();
            while (its.MoveNext())
            {
                its.Current.Value.Send(cmd, buffer);
            }
        }

        public void OnClientConnect(IChannel channel)
        {
            Debug.Log("Server: Client连上服务器");

            long connectionId = channel.ChannelId;
            if (!m_Connections.ContainsKey(connectionId))
            {
                Connection connection = new Connection(channel);
                m_Connections.Add(connectionId, connection);
                RegisterProtocols(connection);
            }
        }

        public void OnClientDisconnect(IChannel channel)
        {
            Debug.Log("Server: Client断开服务器");

            Connection connection;
            long connectionId = channel.ChannelId;
            if (m_Connections.TryGetValue(connectionId, out connection))
            {
                m_Connections.Remove(connectionId);
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
﻿using Zyq.Game.Base;
using System.Collections.Generic;

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

        public override void OnInit()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            ClearConnections();
            m_Network.Dispose();
            m_Connections.Clear();
        }

        public override void OnUpdate(float delta)
        {
            m_Network.OnUpdate();
            base.OnUpdate(delta);
            SyncAttributeMgr.OnUpdate(this, m_Entities.Entitys, delta);
        }

        public void Bind(int port)
        {
            m_Network.Bind(port);
        }

        public void CloseConnection(long connectionId)
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
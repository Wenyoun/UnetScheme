using Zyq.Game.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Game.Server
{
    public class World : IWorld, IServerCallback
    {
        private int m_WorldId;
        private Entities m_Entities;
        private TimerMgr m_TimerMgr;
        private UpdateMgr m_UpdateMgr;
        private MessageMgr m_MessageMgr;
        private ServerNetwork m_Network;
        private SyncAttributeMgr m_SyncAttributeMgr;
        private Dictionary<long, Connection> m_Connections;

        public World()
        {
            m_WorldId = 1;
            m_Entities = new Entities(this);
            m_TimerMgr = new TimerMgr();
            m_UpdateMgr = new UpdateMgr();
            m_MessageMgr = new MessageMgr();
            m_Network = new ServerNetwork(this);
            m_SyncAttributeMgr = new SyncAttributeMgr(this);
            m_Connections = new Dictionary<long, Connection>();
        }

        public void Dispose()
        {
            ClearConnections();
            m_Network.Dispose();
            m_TimerMgr.Dispose();
            m_UpdateMgr.Dispose();
            m_MessageMgr.Dispose();
            m_SyncAttributeMgr.Dispose();
            m_Entities.Dispose();
            m_Connections.Clear();
        }

        public void AddEntity(Entity entity)
        {
            m_Entities.AddEntity(entity);
        }

        public void RemoveEntity(uint eid)
        {
            m_Entities.RemoveEntity(eid);
        }

        public Entity GetEntity(uint entityId)
        {
            return m_Entities.GetEntity(entityId);
        }

        public void Dispatcher(int msgId)
        {
            m_Entities.Dispatcher(msgId, 0, null);
        }

        public void Dispatcher(int mid, uint eid)
        {
            m_Entities.Dispatcher(mid, eid, null);
        }

        public void Dispatcher(int mid, IBody body)
        {
            m_Entities.Dispatcher(mid, 0, body);
        }

        public void Dispatcher(int mid, uint eid, IBody body)
        {
            m_Entities.Dispatcher(mid, eid, body);
        }

        public void OnUpdate(float delta)
        {
            m_Network.OnUpdate();
            m_TimerMgr.OnUpdate(delta);
            m_UpdateMgr.OnUpdate(delta);
            m_Entities.OnUpdate(delta);
            m_SyncAttributeMgr.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            m_Entities.OnFixedUpdate(delta);
        }

        public void OnLateUpdate()
        {
            m_UpdateMgr.OnLateUpdate();
        }

        public List<Entity> Entitys
        {
            get { return m_Entities.Entitys; }
        }

        public int WorldId
        {
            get { return m_WorldId; }
        }

        public TimerMgr Timer
        {
            get { return m_TimerMgr; }
        }

        public UpdateMgr Update
        {
            get { return m_UpdateMgr; }
        }

        public MessageMgr Message
        {
            get { return m_MessageMgr; }
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
using System;
using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class ServerEntityMgr : IDisposable
    {
        private Entities m_Entities;
        private Dictionary<Connection, Entity> m_Connections;

        public ServerEntityMgr()
        {
            m_Entities = new Entities();
            m_Connections = new Dictionary<Connection, Entity>();
        }

        public void Dispose()
        {
            m_Entities.Dispose();
            m_Connections.Clear();
        }

        public void AddEntity(Entity entity)
        {
            if (m_Entities != null)
            {
                m_Entities.AddEntity(entity);
                ConnectionFeture connection = entity.GetFeture<ConnectionFeture>();
                if (connection != null)
                {
                    m_Connections.Add(connection.Connection, entity);
                }
            }
        }

        public void RemoveEntity(uint eid)
        {
            if (m_Entities != null)
            {
                Entity entity = m_Entities.GetEntity(eid);
                if (entity != null)
                {
                    ConnectionFeture connection = entity.GetFeture<ConnectionFeture>();
                    if (connection != null && m_Connections.ContainsKey(connection.Connection))
                    {
                        m_Connections.Remove(connection.Connection);
                    }

                    m_Entities.RemoveEntity(eid);
                }
            }
        }

        public Entity GetEntity(Connection conneciton)
        {
            if (m_Entities != null && m_Connections != null)
            {
                Entity entity = null;
                if (m_Connections.TryGetValue(conneciton, out entity))
                {
                    return entity;
                }
            }

            return null;
        }

        public Entity GetEntity(uint eid)
        {
            if (m_Entities != null)
            {
                return m_Entities.GetEntity(eid);
            }

            return null;
        }

        public void Dispatcher(int msgId)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(msgId, 0, null);
            }
        }

        public void Dispatcher(int mid, uint eid)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, eid, null);
            }
        }

        public void Dispatcher(int mid, IBody body)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, 0, body);
            }
        }

        public void Dispatcher(int mid, uint eid, IBody body)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, eid, body);
            }
        }

        public void OnUpdate(float delta)
        {
            if (m_Entities != null)
            {
                m_Entities.OnUpdate(delta);
            }
        }

        public void OnFixedUpdate(float delta)
        {
            if (m_Entities != null)
            {
                m_Entities.OnFixedUpdate(delta);
            }
        }

        public List<Entity> Entitys
        {
            get
            {
                if (m_Entities != null)
                {
                    return m_Entities.Entitys;
                }

                return null;
            }
        }
    }
}
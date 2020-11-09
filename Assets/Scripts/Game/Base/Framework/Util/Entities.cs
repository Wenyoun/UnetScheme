using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Entities : IDisposable, IUpdate, IFixedUpdate
    {
        private IWorld m_World;
        private List<uint> m_Temp;
        private List<Entity> m_EntityLts;
        private Dictionary<uint, Entity> m_EntityDys;

        public Entities(IWorld world)
        {
            m_World = world;
            m_Temp = new List<uint>();
            m_EntityLts = new List<Entity>();
            m_EntityDys = new Dictionary<uint, Entity>();
        }

        public void Dispose()
        {
            m_Temp.Clear();
            m_EntityLts.Clear();
            m_EntityDys.Clear();
        }

        public void OnUpdate(float delta)
        {
            CheckRemoveEntity();
            int length = m_EntityLts.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    m_EntityLts[i].OnUpdate(delta);
                }
            }
        }

        public void OnFixedUpdate(float delta)
        {
            int length = m_EntityLts.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    m_EntityLts[i].OnFixedUpdate(delta);
                }
            }
        }

        public bool AddEntity(Entity entity)
        {
            uint entityId = entity.EntityId;
            if (!m_EntityDys.ContainsKey(entityId))
            {
                m_EntityLts.Add(entity);
                m_EntityDys.Add(entityId, entity);
                entity.World = m_World;
                entity.OnInit();
                return true;
            }

            return false;
        }

        public bool RemoveEntity(uint eid)
        {
            Entity entity;
            if (m_EntityDys.TryGetValue(eid, out entity))
            {
                entity.IsRemove = true;
                m_Temp.Add(entity.EntityId);
                return true;
            }

            return false;
        }

        public Entity GetEntity(uint entityId)
        {
            Entity entity;
            m_EntityDys.TryGetValue(entityId, out entity);
            return entity;
        }

        public void Dispatcher(int msgId, uint entityId, IBody body)
        {
            if (entityId > 0)
            {
                Entity entity;
                if (m_EntityDys.TryGetValue(entityId, out entity))
                {
                    entity.Dispatcher(msgId, body);
                }
            }
            else
            {
                int length = m_Temp.Count;
                if (length > 0)
                {
                    for (int i = 0; i < length; ++i)
                    {
                        Entity entity = m_EntityLts[i];
                        entity.Dispatcher(msgId, body);
                    }
                }
            }
        }

        public List<Entity> Entitys
        {
            get { return m_EntityLts; }
        }

        private void CheckRemoveEntity()
        {
            int length = m_Temp.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    uint entityId = m_Temp[i];
                    Entity entity = GetEntity(entityId);
                    if (entity != null && entity.IsRemove)
                    {
                        entity.OnRemove();
                    }
                }

                m_Temp.Clear();
            }
        }
    }
}
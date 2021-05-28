using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class Entities : IDisposable
    {
        private World m_World;
        private List<uint> m_Removes;
        private HDictionary<uint, Entity> m_Entities;

        public Entities(World world)
        {
            m_World = world;
            m_Removes = new List<uint>();
            m_Entities = new HDictionary<uint, Entity>();
        }

        public void Dispose()
        {
            m_World = null;
            m_Removes.Clear();
            m_Entities.Clear();
        }

        public void OnUpdate(float delta)
        {
            TickRemoveEntities();
            foreach (Entity entity in m_Entities)
            {
                entity.OnUpdate(delta);
            }
        }

        public void AddEntity(Entity entity)
        {
            uint entityId = entity.EntityId;
            if (!m_Entities.ContainsKey(entityId))
            {
                m_Entities.Add(entityId, entity);
                entity.World = m_World;
                entity.OnInit();
            }
        }

        public void RemoveEntity(uint entityId)
        {
            Entity entity = GetEntity(entityId);
            if (entity != null)
            {
                entity.IsDispose = true;
                m_Removes.Add(entityId);
            }
        }

        public Entity GetEntity(uint entityId)
        {
            if (m_Entities.TryGetValue(entityId, out Entity entity))
            {
                if (!entity.IsDispose)
                {
                    return entity;
                }
            }
            return null;
        }

        public bool CopyEntities(List<Entity> list)
        {
            bool result = false;
            foreach (Entity entity in m_Entities)
            {
                if (!entity.IsDispose)
                {
                    result = true;
                    list.Add(entity);
                }
            }
            return result;
        }

        private void TickRemoveEntities()
        {
            int length = m_Removes.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    uint eid = m_Removes[i];
                    Entity entity = GetEntity(eid);
                    if (entity != null)
                    {
                        if (m_Entities.Remove(eid))
                        {
                            entity.OnRemove();
                        }
                    }
                }
                m_Removes.Clear();
            }
        }
    }
}
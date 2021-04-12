using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class Entities : IDisposable
    {
        private World m_World;
        private List<uint> m_Removes;
        private List<Entity> m_EntityList;
        private Dictionary<uint, Entity> m_EntityDict;

        public Entities(World world)
        {
            m_World = world;
            m_Removes = new List<uint>();
            m_EntityList = new List<Entity>();
            m_EntityDict = new Dictionary<uint, Entity>();
        }

        public void Dispose()
        {
            m_World = null;
            m_Removes.Clear();
            m_EntityList.Clear();
            m_EntityDict.Clear();
        }

        public void OnUpdate(float delta)
        {
            TickRemoveEntity();
            int length = m_EntityList.Count;
            for (int i = 0; i < length; ++i)
            {
                m_EntityList[i].OnUpdate(delta);
            }
        }

        public void AddEntity(Entity entity)
        {
            uint entityId = entity.EntityId;
            if (!m_EntityDict.ContainsKey(entityId))
            {
                m_EntityList.Add(entity);
                m_EntityDict.Add(entityId, entity);
                entity.World = m_World;
                entity.OnInit();
            }
        }

        public void RemoveEntity(uint entityId)
        {
            Entity entity = GetEntity(entityId);
            if (entity != null)
            {
                entity.IsRemove = true;
                m_Removes.Add(entityId);
            }
        }

        public Entity GetEntity(uint entityId)
        {
            if (m_EntityDict.TryGetValue(entityId, out Entity entity))
            {
                if (!entity.IsRemove)
                {
                    return entity;
                }
            }
            return null;
        }

        public bool CopyEntities(List<Entity> list)
        {
            int length = m_EntityList.Count;
            for (int i = 0; i < length; ++i)
            {
                Entity entity = m_EntityList[i];
                if (!entity.IsRemove)
                {
                    list.Add(entity);
                }
            }
            return true;
        }

        private void TickRemoveEntity()
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
                        m_EntityDict.Remove(eid);
                        m_EntityList.Remove(entity);
                        entity.OnRemove();
                    }
                }
                m_Removes.Clear();
            }
        }
    }
}
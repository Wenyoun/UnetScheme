using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Entities : IDisposable, IUpdate, IFixedUpdate
    {
        private IWorld m_World;
        private List<uint> m_Removes;
        private List<Entity> m_EntityList;
        private Dictionary<uint, Entity> m_EntityDict;

        public Entities(IWorld world)
        {
            m_World = world;
            m_Removes = new List<uint>();
            m_EntityList = new List<Entity>();
            m_EntityDict = new Dictionary<uint, Entity>();
        }

        public void Dispose()
        {
            m_Removes.Clear();
            m_EntityList.Clear();
            m_EntityDict.Clear();
        }

        public void OnUpdate(float delta)
        {
            CheckRemoveEntity();
            int length = m_EntityList.Count;
            for (int i = 0; i < length; ++i)
            {
                m_EntityList[i].OnUpdate(delta);
            }
        }

        public void OnFixedUpdate(float delta)
        {
            int length = m_EntityList.Count;
            for (int i = 0; i < length; ++i)
            {
                m_EntityList[i].OnFixedUpdate(delta);
            }
        }

        public bool AddEntity(Entity entity)
        {
            uint entityId = entity.EntityId;
            if (!m_EntityDict.ContainsKey(entityId))
            {
                m_EntityList.Add(entity);
                m_EntityDict.Add(entityId, entity);
                entity.World = m_World;
                entity.OnInit();
                return true;
            }
            return false;
        }

        public bool RemoveEntity(uint entityId)
        {
            Entity entity = GetEntity(entityId);
            if (entity != null)
            {
                entity.IsRemove = true;
                m_Removes.Add(entityId);
                return true;
            }
            return false;
        }

        public Entity GetEntity(uint entityId)
        {
            Entity entity;
            m_EntityDict.TryGetValue(entityId, out entity);
            return entity;
        }

        public List<Entity> Entitys
        {
            get { return m_EntityList; }
        }

        private void CheckRemoveEntity()
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
using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Client
{
    public class ClientEntityMgr
    {
        private Entities m_Entities;

        public ClientEntityMgr()
        {
            m_Entities = new Entities();
        }

        public void Dispose()
        {
            m_Entities.Dispose();
            m_Entities = null;
        }

        public void AddEntity(Entity entity)
        {
            if (m_Entities != null)
            {
                m_Entities.AddEntity(entity);
            }
        }

        public void RemoveEntity(uint eid)
        {
            if (m_Entities != null)
            {
                m_Entities.RemoveEntity(eid);
            }
        }

        public Entity GetEntity(uint eid)
        {
            if (m_Entities != null)
            {
                return m_Entities.GetEntity(eid);
            }
            return null;
        }

        public List<Entity> GetGpsEntitys(uint gid)
        {
            if (m_Entities != null)
            {
                return m_Entities.GetGpsEntitys(gid);
            }
            return null;
        }

        public void Dispatcher(int mid)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, 0, null);
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
    }
}
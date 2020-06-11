using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class EntityMgr
    {
        private static Entities m_Entities;

        public static void Init()
        {
            m_Entities = new Entities();

            UpdateMgr.RegisterUpdate(OnUpdate);
            UpdateMgr.RegisterFixedUpdate(OnFixedUpdate);
        }

        public static void Dispose()
        {
            m_Entities.Dispose();
            m_Entities = null;

            UpdateMgr.UnregisterUpdate(OnUpdate);
            UpdateMgr.UnregisterFixedUpdate(OnFixedUpdate);
        }

        public static void AddEntity(Entity entity)
        {
            if (m_Entities != null)
            {
                m_Entities.AddEntity(entity);
            }
        }

        public static void RemoveEntity(uint eid)
        {
            if (m_Entities != null)
            {
                m_Entities.RemoveEntity(eid);
            }
        }

        public static Entity GetEntity(uint eid)
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

        public static void Dispatcher(int mid)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, 0, null);
            }
        }

        public static void Dispatcher(int mid, uint eid)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, eid, null);
            }
        }

        public static void Dispatcher(int mid, IBody body)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, 0, body);
            }
        }

        public static void Dispatcher(int mid, uint eid, IBody body)
        {
            if (m_Entities != null)
            {
                m_Entities.Dispatcher(mid, eid, body);
            }
        }

        private static void OnUpdate(float delta)
        {
            if (m_Entities != null)
            {
                m_Entities.OnUpdate(delta);
            }
        }

        private static void OnFixedUpdate(float delta)
        {
            if (m_Entities != null)
            {
                m_Entities.OnFixedUpdate(delta);
            }
        }
    }
}
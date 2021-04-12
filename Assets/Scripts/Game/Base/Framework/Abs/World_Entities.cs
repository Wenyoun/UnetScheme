using System.Collections.Generic;

namespace Nice.Game.Base
{
    public abstract partial class World
    {
        public void AddEntity(Entity entity)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Entities.AddEntity(entity);
        }

        public void RemoveEntity(uint eid)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Entities.RemoveEntity(eid);
        }

        public Entity GetEntity(uint eid)
        {
            if (m_Dispose)
            {
                return null;
            }
            return m_Entities.GetEntity(eid);
        }

        public bool CopyEntities(List<Entity> list)
        {
            if (m_Dispose)
            {
                return false;
            }
            return m_Entities.CopyEntities(list);
        }
    }
}
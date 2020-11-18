using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public sealed class WorldLogicMgr : IDisposable
    {
        private IWorld m_World;
        private List<IWorldLogic> m_WorldLogics;

        public WorldLogicMgr(IWorld world)
        {
            m_World = world;
            m_WorldLogics = new List<IWorldLogic>();
        }

        public void Dispose()
        {
            int length = m_WorldLogics.Count;
            for (int i = 0; i < length; ++i)
            {
                m_WorldLogics[i].OnRemove();
            }

            m_WorldLogics.Clear();
        }

        public void AddLogic<T>() where T : IWorldLogic, new()
        {
            if (Find<T>() == null)
            {
                IWorldLogic worldLogic = new T();
                m_WorldLogics.Add(worldLogic);
                worldLogic.OnInit(m_World);
            }
        }

        public void RemoveLogic<T>() where T : IWorldLogic, new()
        {
            IWorldLogic worldLogic = Find<T>();
            if (worldLogic != null)
            {
                m_WorldLogics.Remove(worldLogic);
                worldLogic.OnRemove();
            }
        }

        private IWorldLogic Find<T>()
        {
            Type type = typeof(T);
            IWorldLogic worldLogic = null;

            int length = m_WorldLogics.Count;
            for (int i = 0; i < length; ++i)
            {
                IWorldLogic wlogic = m_WorldLogics[i];
                if (type == wlogic.GetType())
                {
                    worldLogic = wlogic;
                    break;
                }
            }

            return worldLogic;
        }
    }
}
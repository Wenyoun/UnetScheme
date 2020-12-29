﻿using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public sealed class WorldLogicManager : IDisposable
    {
        private IWorld m_World;
        private List<IWorldLogic> m_Logics;

        public WorldLogicManager(IWorld world)
        {
            m_World = world;
            m_Logics = new List<IWorldLogic>();
        }

        public void Dispose()
        {
            int length = m_Logics.Count;
            for (int i = 0; i < length; ++i)
            {
                m_Logics[i].OnRemove();
            }

            m_Logics.Clear();
        }

        public void AddLogic<T>() where T : IWorldLogic, new()
        {
            if (Find<T>() == null)
            {
                IWorldLogic worldLogic = new T();
                m_Logics.Add(worldLogic);
                worldLogic.OnInit(m_World);
            }
        }

        public void RemoveLogic<T>() where T : IWorldLogic, new()
        {
            IWorldLogic worldLogic = Find<T>();
            if (worldLogic != null)
            {
                m_Logics.Remove(worldLogic);
                worldLogic.OnRemove();
            }
        }

        private IWorldLogic Find<T>()
        {
            Type type = typeof(T);
            IWorldLogic worldLogic = null;

            int length = m_Logics.Count;
            for (int i = 0; i < length; ++i)
            {
                IWorldLogic wlogic = m_Logics[i];
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
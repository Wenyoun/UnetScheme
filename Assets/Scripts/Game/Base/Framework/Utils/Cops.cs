using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class Cops : IDisposable
    {
        private HDictionary<Type, ICop> m_Cops;

        public Cops()
        {
            m_Cops = new HDictionary<Type, ICop>();
        }

        public void Dispose()
        {
            foreach (ICop cop in m_Cops)
            {
                cop.OnRemove();
            }
            m_Cops.Clear();
        }

        public T AddCop<T>(ICop cop, Entity entity) where T : ICop
        {
            Type type = cop.GetType();
            if (!m_Cops.ContainsKey(type))
            {
                m_Cops.Add(type, cop);
                cop.Entity = entity;
                cop.World = entity.World;
                cop.OnInit();
            }

            return (T) cop;
        }

        public T AddCop<T>(Entity entity) where T : ICop, new()
        {
            Type type = typeof(T);
            if (!m_Cops.ContainsKey(type))
            {
                T cop = new T();
                m_Cops.Add(type, cop);
                cop.Entity = entity;
                cop.World = entity.World;
                cop.OnInit();
                return cop;
            }

            return default(T);
        }

        public void RemoveCop<T>() where T : ICop
        {
            Type type = typeof(T);
            if (m_Cops.TryGetValue(type, out ICop cop))
            {
                m_Cops.Remove(type);
                cop.OnRemove();
            }
        }

        public T GetCop<T>() where T : ICop
        {
            if (m_Cops.TryGetValue(typeof(T), out ICop cop))
            {
                return (T) cop;
            }
            return default;
        }
    }
}
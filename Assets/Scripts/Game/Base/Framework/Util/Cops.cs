using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Cops : IDisposable
    {
        private List<ICop> m_Temp;
        private List<ICop> m_CopLts;
        private Dictionary<Type, ICop> m_CopDys;

        public Cops()
        {
            m_Temp = new List<ICop>();
            m_CopLts = new List<ICop>();
            m_CopDys = new Dictionary<Type, ICop>();
        }

        public void Dispose()
        {
            RemoveCops();
        }

        public T AddCop<T>(ICop cop, IEntity entity) where T : ICop
        {
            Type type = cop.GetType();
            if (!m_CopDys.ContainsKey(type))
            {
                m_CopLts.Add(cop);
                m_CopDys.Add(type, cop);
                cop.Entity = entity;
                cop.OnInit();
            }

            return (T) cop;
        }

        public T AddCop<T>(IEntity entity) where T : ICop, new()
        {
            Type type = typeof(T);
            if (!m_CopDys.ContainsKey(type))
            {
                T cop = new T();
                m_CopLts.Add(cop);
                m_CopDys.Add(type, cop);
                cop.Entity = entity;
                cop.OnInit();
                return cop;
            }

            return default(T);
        }

        public void RemoveCop<T>() where T : ICop
        {
            ICop cop;
            Type type = typeof(T);
            if (m_CopDys.TryGetValue(type, out cop))
            {
                m_CopLts.Remove(cop);
                m_CopDys.Remove(type);
                cop.OnRemove();
            }
        }

        public void RemoveCops()
        {
            m_Temp.Clear();
            m_Temp.AddRange(m_CopLts);
            for (int i = 0; i < m_Temp.Count; ++i)
            {
                m_Temp[i].OnRemove();
            }

            m_Temp.Clear();
            m_CopLts.Clear();
            m_CopDys.Clear();
        }

        public T GetCop<T>() where T : ICop
        {
            ICop cop = null;
            m_CopDys.TryGetValue(typeof(T), out cop);
            return (T) cop;
        }
    }
}
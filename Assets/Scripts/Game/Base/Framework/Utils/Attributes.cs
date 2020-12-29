using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class Attributes : IDisposable
    {
        private List<IAttribute> m_AttrLts;
        private Dictionary<Type, IAttribute> m_AttrDys;

        public Attributes()
        {
            m_AttrLts = new List<IAttribute>();
            m_AttrDys = new Dictionary<Type, IAttribute>();
        }

        public void Dispose()
        {
            m_AttrLts.Clear();
            m_AttrDys.Clear();
        }

        public T AddAttribute<T>(T attribute) where T : IAttribute
        {
            Type type = attribute.GetType();
            if (!m_AttrDys.ContainsKey(type))
            {
                m_AttrLts.Add(attribute);
                m_AttrDys.Add(type, attribute);
                return (T) attribute;
            }

            return default(T);
        }

        public T GetAttribute<T>() where T : IAttribute
        {
            IAttribute attribute = default(T);
            m_AttrDys.TryGetValue(typeof(T), out attribute);
            return (T) attribute;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Attributes : IDisposable
    {
        private List<IAttribute> mAttrLts;
        private Dictionary<Type, IAttribute> mAttrDys;

        public Attributes()
        {
            mAttrLts = new List<IAttribute>();
            mAttrDys = new Dictionary<Type, IAttribute>();
        }

        public void OnInit()
        {
            Dispose();
        }

        public void Dispose()
        {
            mAttrLts.Clear();
            mAttrDys.Clear();
        }

        public T AddAttribute<T>(T attribute) where T : IAttribute
        {
            Type type = attribute.GetType();
            if (!mAttrDys.ContainsKey(type))
            {
                mAttrLts.Add(attribute);
                mAttrDys.Add(type, attribute);
                return (T)attribute;
            }
            return default(T);
        }

        public T GetAttribute<T>() where T : IAttribute
        {
            IAttribute attribute = default(T);
            mAttrDys.TryGetValue(typeof(T), out attribute);
            return (T)attribute;
        }
    }
}
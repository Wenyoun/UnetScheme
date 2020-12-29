using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class SyncAttributes : IDisposable
    {
        private List<ISyncAttribute> m_AttrLts;
        private Dictionary<long, ISyncAttribute> m_AttrIdDys;
        private Dictionary<Type, ISyncAttribute> m_AttrTypeDys;

        public SyncAttributes()
        {
            m_AttrLts = new List<ISyncAttribute>();
            m_AttrIdDys = new Dictionary<long, ISyncAttribute>();
            m_AttrTypeDys = new Dictionary<Type, ISyncAttribute>();
        }

        public void OnInit()
        {
            m_AttrLts.Clear();
            m_AttrIdDys.Clear();
            m_AttrTypeDys.Clear();
        }

        public void Dispose()
        {
            m_AttrLts.Clear();
            m_AttrIdDys.Clear();
            m_AttrTypeDys.Clear();
        }

        public T AddAttribute<T>(T attribute) where T : ISyncAttribute
        {
            long syncId = attribute.GetSyncId();
            if (!m_AttrIdDys.ContainsKey(syncId))
            {
                m_AttrLts.Add(attribute);
                m_AttrIdDys.Add(syncId, attribute);
                m_AttrTypeDys.Add(typeof(T), attribute);
                return (T) attribute;
            }

            return default(T);
        }

        public T GetSyncAttribute<T>() where T : ISyncAttribute
        {
            ISyncAttribute attribute = default(T);
            m_AttrTypeDys.TryGetValue(typeof(T), out attribute);
            return (T) attribute;
        }

        public ISyncAttribute GetSyncAttribute(uint syncId)
        {
            ISyncAttribute attribute = null;
            m_AttrIdDys.TryGetValue(syncId, out attribute);
            return attribute;
        }

        public List<ISyncAttribute> Attributes
        {
            get { return m_AttrLts; }
        }
    }
}
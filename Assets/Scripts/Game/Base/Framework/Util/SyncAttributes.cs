using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class SyncAttributes : IDisposable
    {
        private List<ISyncAttribute> mAttrLts;
        private Dictionary<uint, ISyncAttribute> mAttrDys;

        public SyncAttributes()
        {
            mAttrLts = new List<ISyncAttribute>();
            mAttrDys = new Dictionary<uint, ISyncAttribute>();
        }

        public void OnInit()
        {
            mAttrLts.Clear();
            mAttrDys.Clear();
        }

        public void Dispose()
        {
            mAttrLts.Clear();
            mAttrDys.Clear();
        }

        public T AddAttribute<T>(T attribute) where T : ISyncAttribute
        {
            if (!mAttrDys.ContainsKey(attribute.SyncId))
            {
                mAttrLts.Add(attribute);
                mAttrDys.Add(attribute.SyncId, attribute);
                return (T)attribute;
            }
            return default(T);
        }

        public T GetSyncAttribute<T>(uint syncId) where T : ISyncAttribute
        {
            ISyncAttribute attribute = default(T);
            mAttrDys.TryGetValue(syncId, out attribute);
            return (T)attribute;
        }

        public List<ISyncAttribute> ALL { get { return mAttrLts; } }
    }
}
using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class SyncAttributes : IDisposable
    {
        private List<ISyncAttribute> mAttrLts;
        private Dictionary<uint, ISyncAttribute> mAttrIdDys;
        private Dictionary<Type, ISyncAttribute> mAttrTypeDys;

        public SyncAttributes()
        {
            mAttrLts = new List<ISyncAttribute>();
            mAttrIdDys = new Dictionary<uint, ISyncAttribute>();
            mAttrTypeDys = new Dictionary<Type, ISyncAttribute>();
        }

        public void OnInit()
        {
            mAttrLts.Clear();
            mAttrIdDys.Clear();
            mAttrTypeDys.Clear();
        }

        public void Dispose()
        {
            mAttrLts.Clear();
            mAttrIdDys.Clear();
            mAttrTypeDys.Clear();
        }

        public T AddAttribute<T>(T attribute) where T : ISyncAttribute
        {
            if (!mAttrIdDys.ContainsKey(attribute.SyncId))
            {
                mAttrLts.Add(attribute);
                mAttrIdDys.Add(attribute.SyncId, attribute);
                mAttrTypeDys.Add(typeof(T), attribute);
                return (T)attribute;
            }
            return default(T);
        }

        public T GetSyncAttribute<T>() where T : ISyncAttribute
        {
            ISyncAttribute attribute = default(T);
            mAttrTypeDys.TryGetValue(typeof(T), out attribute);
            return (T)attribute;
        }

        public T GetSyncAttribute<T>(uint syncId) where T : ISyncAttribute
        {
            ISyncAttribute attribute = default(T);
            mAttrIdDys.TryGetValue(syncId, out attribute);
            return (T)attribute;
        }

        public List<ISyncAttribute> Attributes { get { return mAttrLts; } }
    }
}
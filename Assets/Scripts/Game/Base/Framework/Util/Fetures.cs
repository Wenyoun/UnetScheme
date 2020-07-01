using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Fetures : IDisposable
    {
        private List<IFeture> mFetLts;
        private Dictionary<Type, IFeture> mFetDys;

        public void OnInit()
        {
            mFetLts = new List<IFeture>();
            mFetDys = new Dictionary<Type, IFeture>();
        }

        public void Dispose()
        {
            mFetLts.Clear();
            mFetDys.Clear();
            mFetLts = null;
            mFetDys = null;
        }

        public T AddFeture<T>(T feture, IEntity entity) where T : IFeture
        {
            Type type = feture.GetType();
            if (!mFetDys.ContainsKey(type))
            {
                mFetLts.Add(feture);
                mFetDys.Add(type, feture);
                return (T)feture;
            }
            return default(T);
        }

        public T GetFeture<T>() where T : IFeture
        {
            IFeture feture = default(T);
            mFetDys.TryGetValue(typeof(T), out feture);
            return (T)feture;
        }
    }
}
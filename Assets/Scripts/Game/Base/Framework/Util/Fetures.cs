using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Fetures : IDisposable
    {
        private List<IFeture> mFetLts;
        private Dictionary<Type, IFeture> mFetDys;

        public Fetures()
        {
            mFetLts = new List<IFeture>();
            mFetDys = new Dictionary<Type, IFeture>();
        }

        public void OnInit()
        {
            Dispose();
        }

        public void Dispose()
        {
            for (int i = 0; i < mFetLts.Count; ++i)
            {
                mFetLts[i].OnRemove();
            }
            mFetLts.Clear();
            mFetDys.Clear();
        }

        public T AddFeture<T>(T feture, IEntity entity) where T : IFeture
        {
            Type type = feture.GetType();
            if (!mFetDys.ContainsKey(type))
            {
                mFetLts.Add(feture);
                mFetDys.Add(type, feture);
                feture.OnInit(entity);
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
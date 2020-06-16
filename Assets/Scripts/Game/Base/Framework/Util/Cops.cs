using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Cops : IDisposable
    {
        private List<ICop> mTemp;
        private List<ICop> mCopLts;
        private Dictionary<Type, ICop> mCopDys;

        public Cops()
        {
            mTemp = new List<ICop>();
            mCopLts = new List<ICop>();
            mCopDys = new Dictionary<Type, ICop>();
        }

        public void OnInit()
        {
            RemoveCops();
        }

        public void Dispose()
        {
            RemoveCops();
        }

        public bool IsNotExCop<T>() where T : ICop
        {
            return !mCopDys.ContainsKey(typeof(T));
        }

        public T AddCop<T>(ICop cop, IEntity entity) where T : ICop
        {
            Type type = cop.GetType();
            if (!mCopDys.ContainsKey(type))
            {
                mCopLts.Add(cop);
                mCopDys.Add(type, cop);
                cop.Entity = entity;
                cop.OnInit();
            }
            return (T)cop;
        }

        public T AddCop<T>(IEntity entity) where T : ICop, new()
        {
            Type type = typeof(T);
            if (!mCopDys.ContainsKey(type))
            {
                T cop = new T();
                mCopLts.Add(cop);
                mCopDys.Add(type, cop);
                cop.Entity = entity;
                cop.OnInit();
                return cop;
            }
            return default(T);
        }

        public void RemoveCop<T>() where T : ICop
        {
            ICop cop = null;
            Type type = typeof(T);
            if (mCopDys.TryGetValue(type, out cop))
            {
                mCopLts.Remove(cop);
                mCopDys.Remove(type);
                cop.OnRemove();
            }
        }

        public void RemoveCops()
        {
            mTemp.Clear();
            mTemp.AddRange(mCopLts);
            for (int i = 0; i < mTemp.Count; ++i)
            {
                mTemp[i].OnRemove();
            }
            mTemp.Clear();
            mCopLts.Clear();
            mCopDys.Clear();
        }

        public T GetCop<T>() where T : ICop
        {
            ICop cop = null;
            mCopDys.TryGetValue(typeof(T), out cop);
            return (T)cop;
        }
    }
}
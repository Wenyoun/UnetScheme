using System.Collections.Generic;

namespace Base
{
    internal class Model : IModel
    {
        private Dictionary<System.Type, IProxy> mProxys;

        public Model()
        {
            mProxys = new Dictionary<System.Type, IProxy>();
        }

        public void RegisterProxy(IProxy proxy)
        {
            if (!mProxys.ContainsKey(proxy.GetType()))
            {
                mProxys.Add(proxy.GetType(), proxy);
                proxy.OnRegister();
            }
            else
            {
                throw new System.ArgumentException("Type=" + proxy.GetType() + " Already Register!");
            }
        }

        public T RetrieveProxy<T>() where T : IProxy
        {
            IProxy proxy = null;
            mProxys.TryGetValue(typeof(T), out proxy);
            return (T)proxy;
        }

        public IProxy RemoveProxy(System.Type type)
        {
            IProxy proxy = null;
            if (mProxys.TryGetValue(type, out proxy))
            {
                mProxys.Remove(type);
                proxy.OnRemove();
            }
            return proxy;
        }

        public bool HasProxy<T>() where T : IProxy
        {
            return mProxys.ContainsKey(typeof(T));
        }
    }
}
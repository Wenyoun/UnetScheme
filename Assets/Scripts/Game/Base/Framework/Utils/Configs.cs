using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class Configs : IDisposable
    {
        private List<IConfig> mConfigLts;
        private Dictionary<Type, IConfig> mConfigDys;

        public Configs()
        {
            mConfigLts = new List<IConfig>();
            mConfigDys = new Dictionary<Type, IConfig>();
        }

        public void Dispose()
        {
            mConfigLts.Clear();
            mConfigDys.Clear();
        }

        public T AddConfig<T>(T config) where T : IConfig
        {
            Type type = config.GetType();
            if (!mConfigDys.ContainsKey(type))
            {
                mConfigLts.Add(config);
                mConfigDys.Add(type, config);
                return (T) config;
            }

            return default(T);
        }

        public T GetConfig<T>() where T : IConfig
        {
            IConfig config = default(T);
            mConfigDys.TryGetValue(typeof(T), out config);
            return (T) config;
        }

        public List<IConfig> Copy()
        {
            List<IConfig> configs = new List<IConfig>();
            for (int i = 0; i < mConfigLts.Count; ++i)
            {
                configs.Add(mConfigLts[i].Copy());
            }

            return configs;
        }
    }
}
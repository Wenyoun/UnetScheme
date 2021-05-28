using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public sealed class FeatureRegister : IDisposable
    {
        private AbsWorld m_World;
        private List<IWorldFeature> m_Features;

        public FeatureRegister(AbsWorld world)
        {
            m_World = world;
            m_Features = new List<IWorldFeature>();
        }

        public void Dispose()
        {
            int length = m_Features.Count;
            for (int i = 0; i < length; ++i)
            {
                m_Features[i].OnRemove();
            }
            m_Features.Clear();
        }

        public void AddFeature<T>() where T : IWorldFeature, new()
        {
            if (FindFeature<T>() == null)
            {
                IWorldFeature feature = new T();
                m_Features.Add(feature);
                feature.OnInit(m_World);
            }
        }

        public void RemoveFeature<T>() where T : IWorldFeature, new()
        {
            IWorldFeature feature = FindFeature<T>();
            if (feature != null)
            {
                m_Features.Remove(feature);
                feature.OnRemove();
            }
        }

        private IWorldFeature FindFeature<T>()
        {
            Type type = typeof(T);
            int length = m_Features.Count;
            for (int i = 0; i < length; ++i)
            {
                IWorldFeature feature = m_Features[i];
                if (type == feature.GetType())
                {
                    return feature;
                }
            }
            return null;
        }
    }
}
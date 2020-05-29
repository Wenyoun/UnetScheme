using Base;
using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    //特效缓存池
    public class FxCachedPool : ILifecycle
    {
        private GameObject m_Root;
        private Dictionary<string, FxCached> m_Cacheds;

        public void OnInit()
        {
            m_Root = new GameObject("Fxs");
            m_Root.transform.position = new Vector3(99999, 99999, 0);
            m_Cacheds = new Dictionary<string, FxCached>();
        }

        public void OnRemove()
        {
            foreach (FxCached cached in m_Cacheds.Values)
            {
                cached.Dispose();
            }
            m_Cacheds.Clear();
            m_Cacheds = null;
            Object.Destroy(m_Root);
            m_Root = null;
        }

        public Fx Take(string path, Transform parent)
        {
            FxCached cached = null;
            if (!m_Cacheds.TryGetValue(path, out cached))
            {
                cached = new FxCached(path);
                m_Cacheds.Add(path, cached);
            }
            return cached.Take(parent);
        }

        public void Recycle(Fx fx)
        {
            if (fx != null)
            {
                FxCached cached = null;
                if (m_Cacheds.TryGetValue(fx.Path, out cached))
                {
                    cached.Recycle(fx, m_Root.transform);
                }
            }
        }
    }
}
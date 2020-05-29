using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    public class FxCached : System.IDisposable
    {
        private string m_Path;
        public Queue<Fx> m_List;

        public FxCached(string path)
        {
            m_Path = path;
            m_List = new Queue<Fx>();
        }

        public Fx Take(Transform parent)
        {
            Fx fx = null;
            if (m_List.Count == 0)
            {
                fx = Create(parent);
            }
            else
            {
                fx = m_List.Dequeue();
                fx.transform.SetParent(parent);
            }

            return fx;
        }

        public void Recycle(Fx fx, Transform parent)
        {
            m_List.Enqueue(fx);
            fx.transform.SetParent(parent);
            fx.transform.localPosition = Vector3.zero;
            fx.transform.localRotation = Quaternion.identity;
            fx.transform.localScale = Vector3.one;

            fx.Stop();
        }

        public void Dispose()
        {
            while (m_List.Count > 0)
            {
                Object.Destroy(m_List.Dequeue());
            }
            m_List.Clear();
        }

        private Fx Create(Transform parent)
        {
            Object prefab = Resources.Load(m_Path);
            if (prefab != null)
            {
                GameObject root = Object.Instantiate(prefab) as GameObject;
                root.name = prefab.name;
                if (parent != null)
                {
                    root.transform.SetParent(parent);
                }
                root.transform.localPosition = Vector3.zero;
                root.transform.localRotation = Quaternion.identity;
                root.transform.localScale = Vector3.one;
                Fx fx = root.AddComponent<Fx>();
                fx.Path = m_Path;
                return fx;
            }
            else
            {
                Debug.Log("找不到path=" + m_Path + "的资源.");
            }
            return null;
        }
    }
}
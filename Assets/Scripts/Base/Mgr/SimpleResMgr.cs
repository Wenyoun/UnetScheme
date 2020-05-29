using UnityEngine;
using System.Collections.Generic;

namespace Base
{
    public sealed class SimpleResMgr
    {
        public static GameObject Instantiate(GameObject prefab, Transform parent = null)
        {
            GameObject root = Object.Instantiate(prefab) as GameObject;
            root.name = prefab.name;
            if (parent != null)
            {
                root.transform.SetParent(parent);
            }
            return root;
        }

        public static GameObject CreateRoot(string path, Transform parent = null)
        {
            Object prefab = Resources.Load(path);
            if (prefab != null)
            {
                GameObject root = Object.Instantiate(prefab) as GameObject;
                root.name = prefab.name;
                if (parent != null)
                {
                    root.transform.SetParent(parent);
                }
                return root;
            }
            else
            {
                Debug.Log("找不到path=" + path + "的资源.");
            }
            return null;
        }

        public static GameObject CreateViewRoot(string path, Transform parent)
        {
            Object prefab = Resources.Load(path);
            if (prefab != null)
            {
                GameObject root = Object.Instantiate(prefab) as GameObject;
                root.transform.SetParent(parent);
                root.transform.localPosition = Vector3.zero;
                root.transform.localRotation = Quaternion.identity;
                root.transform.localScale = Vector3.one;
                root.name = prefab.name;
                RectTransform r = root.transform as RectTransform;
                if (r != null)
                {
                    r.offsetMin = Vector2.zero;
                    r.offsetMax = Vector2.zero;
                    r.anchorMin = Vector2.zero;
                    r.anchorMax = Vector2.one;
                }
                root.SetActive(false);
                return root;
            }
            else
            {
                Debug.Log("找不到path=" + path + "的资源.");
            }
            return null;
        }

        public static GameObject InstantiateGo(GameObject org, Transform parent, string name = "clone", bool inWorld = false, bool active = true)
        {
            GameObject go = Object.Instantiate(org, parent, inWorld);
            go.name = name;
            go.SetActive(active);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }
            return go;
        }

        public static List<GameObject> InsFactory(GameObject org, Transform parent, int count, int startIndex = 1, string nameSuffix = "clone", bool active = true)
        {
            List<GameObject> ret = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject go = Object.Instantiate(org, parent, false);
                if (go.transform is RectTransform)
                {
                    RectTransform rect = go.transform as RectTransform;
                    rect.anchoredPosition3D = Vector3.zero;
                }
                else
                {
                    go.transform.localPosition = Vector3.zero;
                }
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.name = nameSuffix + (i + startIndex);
                ret.Add(go);
                go.SetActive(active);
            }
            return ret;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

namespace Base
{
    public class ViewComponent : MonoBehaviour
    {
        [SerializeField]
        private List<Pair> Pairs = new List<Pair>();
        private Dictionary<string, object> Cops = new Dictionary<string, object>();

        private void Awake()
        {
            foreach (Pair pair in Pairs)
            {
                if (pair.Value != null)
                {
                    Cops.Add(pair.Key, pair.Value);
                }
            }
        }

        private void OnDestroy()
        {
            Pairs.Clear();
            Cops.Clear();
        }

        public T Get<T>(string key)
        {
            object value = null;
            Cops.TryGetValue(key, out value);
#if UNITY_EDITOR
            try
            {
                return (T)value;
            }
            catch (System.Exception e)
            {
                Debug.Log(typeof(T));
                Debug.LogError("key为" + key + ",在view为" + gameObject.name + "中的类型出错->" + e.ToString());
                return default(T);
            }
#else
            return (T)value;
#endif
        }

        public enum Cop
        {
            GameObject = 0,
            RectTransform,
            Sprite,
            Text,
            Image,
            RawImage,
            Button,
            Toggle,
            Slider,
            Scrollbar,
            Dropdown,
            InputField,
            ScrollRect,
            Canvas,
            CanvasGroup,
            Transform,
        }

        [System.Serializable]
        public struct Pair
        {
            public Cop Type;
            public string Key;
            public Object Value;
        }
    }
}

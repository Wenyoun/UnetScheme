using UnityEngine;

namespace Base
{
    public sealed class GlobalMgr
    {
        public static Camera UI;
        public static GameObject Root;
        public static MonoBehaviour Behaviour;

        public static void Config()
        {
            Root = GameObject.Find("Engine");
            Behaviour = Root.GetComponent<Engine>();
            UI = Root.GetComponentInChildren<Camera>();
            GameObject.DontDestroyOnLoad(Root);
        }
    }
}

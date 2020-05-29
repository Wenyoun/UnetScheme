using UnityEngine;

namespace Game
{
    //特效管理器
    public class FxMgr
    {
        public static Fx Shot(string path, Vector3 position, Transform parent = null)
        {
            Fx fx = Create(path, position, parent);
            if (fx != null)
            {
                fx.Play(true);
            }
            return fx;
        }

        private static Fx Create(string path, Vector3 position, Transform parent = null)
        {
            Fx fx = CachedMgr.Fx.Take(path, parent);
            fx.transform.localPosition = position;
            fx.transform.localRotation = Quaternion.identity;
            fx.transform.localScale = Vector3.one;
            return fx;
        }
    }
}
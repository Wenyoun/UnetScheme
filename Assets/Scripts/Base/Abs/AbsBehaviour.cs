using Nice.Game.Base;
using System.Collections.Generic;

namespace Base
{
    public abstract class AbsBehaviour : AbsCompose
    {
        private List<ICompose> mModuleLts;
        private Dictionary<System.Type, ICompose> mModuleDys;

        public AbsBehaviour()
        {
            mModuleLts = new List<ICompose>();
            mModuleDys = new Dictionary<System.Type, ICompose>();
        }

        public override void OnInit() { }

        public override void OnRemove()
        {
            for (int i = 0; i < mModuleLts.Count; ++i)
            {
                mModuleLts[i].OnRemove();
            }
            mModuleLts.Clear();
            mModuleDys.Clear();
        }

        public override void OnUpdate(float delta)
        {
            for (int i = 0; i < mModuleLts.Count; ++i)
            {
                mModuleLts[i].OnUpdate(delta);
            }
        }

        public override void OnLateUpdate(float delta)
        {
            for (int i = 0; i < mModuleLts.Count; ++i)
            {
                mModuleLts[i].OnLateUpdate(delta);
            }
        }

        public override void OnFixedUpdate(float delta)
        {
            for (int i = 0; i < mModuleLts.Count; ++i)
            {
                mModuleLts[i].OnFixedUpdate(delta);
            }
        }

        public T Add<T>() where T : ICompose, new()
        {
            System.Type type = typeof(T);
            if (!mModuleDys.ContainsKey(type))
            {
                T module = new T();
                mModuleLts.Add(module);
                mModuleDys.Add(type, module);
                module.OnInit();
                return module;
            }
            return default(T);
        }

        public void Remove<T>() where T : ICompose
        {
            System.Type type = typeof(T);
            ICompose module = null;
            if (mModuleDys.TryGetValue(type, out module))
            {
                mModuleLts.Remove(module);
                mModuleDys.Remove(type);
                module.OnRemove();
            }
        }
    }
}
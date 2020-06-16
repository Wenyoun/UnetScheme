using Zyq.Game.Base;

namespace Base
{
    public abstract class AbsCompose : ICompose
    {
        public virtual void OnInit() { }

        public virtual void OnRemove() { }

        public virtual void OnUpdate(float delta) { }

        public virtual void OnLateUpdate() { }

        public virtual void OnFixedUpdate(float delta) { }
    }
}
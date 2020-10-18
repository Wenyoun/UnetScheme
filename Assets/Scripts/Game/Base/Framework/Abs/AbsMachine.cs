namespace Zyq.Game.Base
{
    public abstract class AbsMachine : ILifecycle, IUpdate, IFixedUpdate
    {
        public TimerMgr TimerMgr { get; private set; }
        public UpdateMgr UpdateMgr { get; private set; }
        public MessageMgr MessageMgr { get; private set; }

        public virtual void OnInit()
        {
            TimerMgr = new TimerMgr();
            UpdateMgr = new UpdateMgr();
            MessageMgr = new MessageMgr();
        }

        public virtual void OnRemove()
        {
            TimerMgr.Dispose();
            MessageMgr.Dispose();
            UpdateMgr.Dispose();
        }

        public virtual void OnUpdate(float delta)
        {
            if (TimerMgr != null)
            {
                TimerMgr.OnUpdate(delta);
            }
            if (UpdateMgr != null)
            {
                UpdateMgr.OnUpdate(delta);
            }
        }

        public virtual void OnFixedUpdate(float delta)
        {
            if (UpdateMgr != null)
            {
                UpdateMgr.OnFixedUpdate(delta);
            }
        }
    }
}
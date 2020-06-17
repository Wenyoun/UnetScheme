namespace Zyq.Game.Base
{
    public class AbsMachine : ILifecycle, IUpdate, IFixedUpdate
    {
        public TimerMgr TimerMgr { get; private set; }
        public EntityMgr EntityMgr { get; private set; }
        public UpdateMgr UpdateMgr { get; private set; }
        public MessageMgr MessageMgr { get; private set; }

        public AbsMachine()
        {
            TimerMgr = new TimerMgr();
            EntityMgr = new EntityMgr();
            UpdateMgr = new UpdateMgr();
            MessageMgr = new MessageMgr();
        }

        public virtual void OnInit()
        {
            UpdateMgr.Init();
            MessageMgr.Init();
            TimerMgr.Init();
            EntityMgr.Init();
        }

        public virtual void OnRemove()
        {
            EntityMgr.Dispose();
            TimerMgr.Dispose();
            MessageMgr.Dispose();
            UpdateMgr.Dispose();
        }

        public virtual void OnUpdate(float delta)
        {
            TimerMgr.OnUpdate(delta);
            UpdateMgr.OnUpdate(delta);
            EntityMgr.OnUpdate(delta);
        }

        public virtual void OnFixedUpdate(float delta)
        {
            UpdateMgr.OnFixedUpdate(delta);
            EntityMgr.OnFixedUpdate(delta);
        }
    }
}
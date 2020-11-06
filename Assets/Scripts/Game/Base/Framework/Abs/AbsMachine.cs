namespace Zyq.Game.Base
{
    public abstract class AbsMachine : ILifecycle, IUpdate, IFixedUpdate
    {
        private TimerMgr m_TimerMgr;
        private UpdateMgr m_UpdateMgr;
        private MessageMgr m_MessageMgr;

        public virtual void OnInit()
        {
            m_TimerMgr = new TimerMgr();
            m_UpdateMgr = new UpdateMgr();
            m_MessageMgr = new MessageMgr();
        }

        public virtual void OnRemove()
        {
            if (m_TimerMgr != null)
            {
                m_TimerMgr.Dispose();
            }

            if (m_UpdateMgr != null)
            {
                m_UpdateMgr.Dispose();
            }

            if (m_MessageMgr != null)
            {
                m_MessageMgr.Dispose();
            }
        }

        public virtual void OnUpdate(float delta)
        {
            if (m_TimerMgr != null)
            {
                m_TimerMgr.OnUpdate(delta);
            }

            if (m_UpdateMgr != null)
            {
                m_UpdateMgr.OnUpdate(delta);
            }
        }

        public virtual void OnFixedUpdate(float delta)
        {
            if (m_UpdateMgr != null)
            {
                m_UpdateMgr.OnFixedUpdate(delta);
            }
        }

        public TimerMgr Timer
        {
            get { return m_TimerMgr; }
        }

        public UpdateMgr Update
        {
            get { return m_UpdateMgr; }
        }

        public MessageMgr Message
        {
            get { return m_MessageMgr; }
        }
    }
}
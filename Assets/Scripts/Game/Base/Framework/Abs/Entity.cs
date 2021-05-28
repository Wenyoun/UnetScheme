namespace Nice.Game.Base
{
    public abstract class Entity
    {
        private bool m_Dispose;
        private uint m_EntityId;
        private AbsWorld m_World;
        
        private Cops m_Cops;
        private Attributes m_Attributes;
        private SyncAttributes m_SyncAttributes;

        private TimerRegister m_Timer;
        private UpdaterRegister m_Updater;

        protected Entity()
        {
            m_Dispose = false;
            m_EntityId = UniGenID.GenNextEntityID();

            m_Cops = new Cops();

            m_Attributes = new Attributes();
            m_SyncAttributes = new SyncAttributes();

            m_Timer = new TimerRegister();
            m_Updater = new UpdaterRegister();
        }

        public T AddCop<T>(T cop) where T : ICop
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_Cops.AddCop<T>(cop, this);
        }

        public T AddCop<T>() where T : ICop, new()
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_Cops.AddCop<T>(this);
        }

        public void RemoveCop<T>() where T : ICop
        {
            if (m_Dispose)
            {
                return;
            }
            m_Cops.RemoveCop<T>();
        }

        public T GetCop<T>() where T : ICop
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_Cops.GetCop<T>();
        }

        public T AddAttribute<T>(T attribute) where T : IAttribute
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_Attributes.AddAttribute<T>(attribute);
        }

        public T GetAttribute<T>() where T : IAttribute
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_Attributes.GetAttribute<T>();
        }

        public T AddSyncAttribute<T>(T attribute) where T : ISyncAttribute
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_SyncAttributes.AddAttribute<T>(attribute);
        }

        public T GetSyncAttribute<T>() where T : ISyncAttribute
        {
            if (m_Dispose)
            {
                return default(T);
            }
            return m_SyncAttributes.GetSyncAttribute<T>();
        }

        public ISyncAttribute GetSyncAttribute(uint syncId)
        {
            if (m_Dispose)
            {
                return null;
            }
            return m_SyncAttributes.GetSyncAttribute(syncId);
        }

        public void OnUpdate(float delta)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Timer.OnUpdate(delta);
            m_Updater.OnUpdate(delta);
        }

        public bool IsDispose
        {
            get { return m_Dispose; }
            internal set { m_Dispose = value; }
        }

        public AbsWorld World
        {
            get { return m_World; }
            internal set { m_World = value; }
        }

        public uint EntityId
        {
            get { return m_EntityId; }
        }

        public SyncAttributes Sync
        {
            get { return m_SyncAttributes; }
        }

        public TimerRegister Timer
        {
            get { return m_Timer; }
        }

        public UpdaterRegister Updater
        {
            get { return m_Updater; }
        }

        public void OnInit()
        {
            Init();
        }

        public void OnRemove()
        {
            Clear();
            m_Dispose = true;
            m_Cops.Dispose();
            m_Attributes.Dispose();
            m_Timer.Dispose();
            m_Updater.Dispose();
        }

        protected virtual void Init()
        {
        }

        protected virtual void Clear()
        {
        }
    }
}
namespace Zyq.Game.Base
{
    public abstract class AbsEntity : IEntity
    {
        private IWorld m_World;
        private bool m_isRemove;
        private uint m_EntityId;

        private Cops m_Cops;
        private Configs m_Configs;
        private Fetures m_Fetures;

        private Attributes m_Attributes;
        private SyncAttributes m_SyncAttributes;

        private TimerRegister m_Timer;
        private UpdaterRegister m_Updater;

        public AbsEntity()
        {
            m_isRemove = false;
            m_EntityId = UniGenID.GenNextEntityID();

            m_Cops = new Cops();
            m_Configs = new Configs();
            m_Fetures = new Fetures();

            m_Attributes = new Attributes();
            m_SyncAttributes = new SyncAttributes();

            m_Timer = new TimerRegister();
            m_Updater = new UpdaterRegister();
        }

        public T AddCop<T>(T cop) where T : ICop
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Cops.AddCop<T>(cop, this);
        }

        public T AddCop<T>() where T : ICop, new()
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Cops.AddCop<T>(this);
        }

        public void RemoveCop<T>() where T : ICop
        {
            if (m_isRemove)
            {
                return;
            }
            m_Cops.RemoveCop<T>();
        }

        public void RemoveCops()
        {
            if (m_isRemove)
            {
                return;
            }
            m_Cops.RemoveCops();
        }

        public T GetCop<T>() where T : ICop
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Cops.GetCop<T>();
        }

        public T AddConfig<T>(T config) where T : IConfig
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Configs.AddConfig<T>(config);
        }

        public T GetConfig<T>() where T : IConfig
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Configs.GetConfig<T>();
        }

        public T AddAttribute<T>(T attribute) where T : IAttribute
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Attributes.AddAttribute<T>(attribute);
        }

        public T GetAttribute<T>() where T : IAttribute
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Attributes.GetAttribute<T>();
        }

        public T AddSyncAttribute<T>(T attribute) where T : ISyncAttribute
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_SyncAttributes.AddAttribute<T>(attribute);
        }

        public T GetSyncAttribute<T>() where T : ISyncAttribute
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_SyncAttributes.GetSyncAttribute<T>();
        }

        public ISyncAttribute GetSyncAttribute(uint syncId)
        {
            if (m_isRemove)
            {
                return null;
            }
            return m_SyncAttributes.GetSyncAttribute(syncId);
        }

        public T AddFeture<T>(T feture) where T : IFeture
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Fetures.AddFeture<T>(feture, this);
        }

        public T GetFeture<T>() where T : IFeture
        {
            if (m_isRemove)
            {
                return default(T);
            }
            return m_Fetures.GetFeture<T>();
        }

        public void OnUpdate(float delta)
        {
            if (m_isRemove)
            {
                return;
            }
            m_Timer.OnUpdate(delta);
            m_Updater.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            if (m_isRemove)
            {
                return;
            }
            m_Updater.OnFixedUpdate(delta);
        }

        public bool IsRemove
        {
            get { return m_isRemove; }
            internal set { m_isRemove = true; }
        }

        public IWorld World
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
            m_Cops.Dispose();
            m_Fetures.Dispose();
            m_Attributes.Dispose();
            m_Configs.Dispose();
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
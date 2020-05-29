namespace Game
{
    public abstract class AbsEntity : IEntity
    {
        private Cops mCops;
        private Fetures mFetures;
        private Configs mConfigs;
        private Attributes mAttributes;

        public AbsEntity()
        {
            mCops = new Cops();
            mFetures = new Fetures();
            mConfigs = new Configs();
            mAttributes = new Attributes();
            MsgRegister = new MsgRegister();
            TimerRegister = new TimerRegister();
            UpdateRegister = new UpdateRegister();
        }

        public T AddCop<T>(T cop) where T : ICop
        {
            return mCops.AddCop<T>(cop, this);
        }

        public T AddCop<T>() where T : ICop, new()
        {
            return mCops.AddCop<T>(this);
        }

        public void RemoveCop<T>() where T : ICop
        {
            mCops.RemoveCop<T>();
        }

        public void RemoveCops()
        {
            mCops.RemoveCops();
        }

        public T GetCop<T>() where T : ICop
        {
            return mCops.GetCop<T>();
        }

        public T AddConfig<T>(T config) where T : IConfig
        {
            return mConfigs.AddConfig<T>(config);
        }

        public T GetConfig<T>() where T : IConfig
        {
            return mConfigs.GetConfig<T>();
        }

        public T AddAttribute<T>(T attribute) where T : IAttribute
        {
            return mAttributes.AddAttribute<T>(attribute);
        }

        public T GetAttribute<T>() where T : IAttribute
        {
            return mAttributes.GetAttribute<T>();
        }

        public T AddFeture<T>(T feture) where T : IFeture
        {
            return mFetures.AddFeture<T>(feture, this);
        }

        public T GetFeture<T>() where T : IFeture
        {
            return mFetures.GetFeture<T>();
        }

        public virtual void OnInit()
        {
            mCops.OnInit();
            mFetures.OnInit();
            mConfigs.OnInit();
            mAttributes.OnInit();
            MsgRegister.OnInit();
            TimerRegister.OnInit();
            UpdateRegister.OnInit();
        }

        public virtual void OnAdd()
        {
        }

        public virtual void OnRemove()
        {
            mCops.Dispose();
            mFetures.Dispose();
            mAttributes.Dispose();
            mConfigs.Dispose();

            MsgRegister.Dispose();
            TimerRegister.Dispose();
            UpdateRegister.Dispose();
        }

        public void OnUpdate(float delta)
        {
            TimerRegister.OnUpdate(delta);
            UpdateRegister.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            UpdateRegister.OnFixedUpdate(delta);
        }

        public void Dispatcher(int id, IBody body = null)
        {
            MsgRegister.Dispatcher(id, body);
        }

        public uint Eid { get; set; }

        public uint Gid{ get; set; }

        public MsgRegister MsgRegister { get; private set; }

        public TimerRegister TimerRegister { get; private set; }

        public UpdateRegister UpdateRegister { get; private set; }
    }
}
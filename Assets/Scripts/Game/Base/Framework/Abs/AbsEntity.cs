namespace Zyq.Game.Base
{
    public abstract class AbsEntity : IEntity
    {
        public AbsEntity()
        {
            Cops = new Cops();
            Fetures = new Fetures();
            Configs = new Configs();
            Attributes = new Attributes();
            MsgRegister = new MsgRegister();
            TimerRegister = new TimerRegister();
            UpdateRegister = new UpdateRegister();
        }

        public T AddCop<T>(T cop) where T : ICop
        {
            return Cops.AddCop<T>(cop, this);
        }

        public T AddCop<T>() where T : ICop, new()
        {
            return Cops.AddCop<T>(this);
        }

        public void RemoveCop<T>() where T : ICop
        {
            Cops.RemoveCop<T>();
        }

        public void RemoveCops()
        {
            Cops.RemoveCops();
        }

        public T GetCop<T>() where T : ICop
        {
            return Cops.GetCop<T>();
        }

        public T AddConfig<T>(T config) where T : IConfig
        {
            return Configs.AddConfig<T>(config);
        }

        public T GetConfig<T>() where T : IConfig
        {
            return Configs.GetConfig<T>();
        }

        public T AddAttribute<T>(T attribute) where T : IAttribute
        {
            return Attributes.AddAttribute<T>(attribute);
        }

        public T GetAttribute<T>() where T : IAttribute
        {
            return Attributes.GetAttribute<T>();
        }

        public T AddFeture<T>(T feture) where T : IFeture
        {
            return Fetures.AddFeture<T>(feture, this);
        }

        public T GetFeture<T>() where T : IFeture
        {
            return Fetures.GetFeture<T>();
        }

        public virtual void OnInit()
        {
            Cops.OnInit();
            Fetures.OnInit();
            Configs.OnInit();
            Attributes.OnInit();
            MsgRegister.OnInit();
            TimerRegister.OnInit();
            UpdateRegister.OnInit();
        }

        public virtual void OnAdd()
        {
        }

        public virtual void OnRemove()
        {
            Cops.Dispose();
            Fetures.Dispose();
            Attributes.Dispose();
            Configs.Dispose();
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

        public Fetures Fetures{get;private set;}

        public Configs Configs{get;private set;}

        public Cops Cops { get; private set; }

        public Attributes Attributes { get; private set; }

        public MsgRegister MsgRegister { get; private set; }

        public TimerRegister TimerRegister { get; private set; }

        public UpdateRegister UpdateRegister { get; private set; }
    }
}
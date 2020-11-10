namespace Zyq.Game.Base
{
    public interface IEntity : ILifecycle, IUpdate, IFixedUpdate, IMessage
    {
        T AddCop<T>(T cop) where T : ICop;

        T AddCop<T>() where T : ICop, new();

        void RemoveCop<T>() where T : ICop;

        void RemoveCops();

        T GetCop<T>() where T : ICop;

        T AddConfig<T>(T config) where T : IConfig;

        T GetConfig<T>() where T : IConfig;

        T AddAttribute<T>(T attribute) where T : IAttribute;

        T GetAttribute<T>() where T : IAttribute;

        T AddSyncAttribute<T>(T attribute) where T : ISyncAttribute;

        T GetSyncAttribute<T>() where T : ISyncAttribute;

        ISyncAttribute GetSyncAttribute(uint syncId);

        T AddFeture<T>(T feture) where T : IFeture;

        T GetFeture<T>() where T : IFeture;

        uint EntityId { get; }
        
        IWorld World { get; }

        MsgRegister Message { get; }

        TimerRegister Timer { get; }

        UpdateRegister Update { get; }
    }
}
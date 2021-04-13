namespace Nice.Game.Base
{
    public interface IEntity : ILifecycle
    {
        T AddCop<T>(T cop) where T : ICop;

        T AddCop<T>() where T : ICop, new();

        void RemoveCop<T>() where T : ICop;

        T GetCop<T>() where T : ICop;

        T AddAttribute<T>(T attribute) where T : IAttribute;

        T GetAttribute<T>() where T : IAttribute;

        T AddSyncAttribute<T>(T attribute) where T : ISyncAttribute;

        T GetSyncAttribute<T>() where T : ISyncAttribute;

        ISyncAttribute GetSyncAttribute(uint syncId);

        uint EntityId { get; }

        World World { get; }

        TimerRegister Timer { get; }

        UpdaterRegister Updater { get; }
    }
}
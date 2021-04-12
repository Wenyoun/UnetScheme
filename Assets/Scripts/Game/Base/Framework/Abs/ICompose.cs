namespace Nice.Game.Base
{
    public interface ILifecycle
    {
        void OnInit();

        void OnRemove();
    }

    public interface IUpdate
    {
        void OnUpdate(float delta);
    }

    public interface ILateUpdate
    {
        void OnLateUpdate(float delta);
    }

    public interface IFixedUpdate
    {
        void OnFixedUpdate(float delta);
    }

    public interface ICanUpdate
    {
        bool IsUpdate { get; set; }
    }

    public interface ICompose : ILifecycle, IUpdate, ILateUpdate, IFixedUpdate
    {
    }
}
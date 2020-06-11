namespace Zyq.Base
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

    public interface IFixedUpdate
    {
        void OnFixedUpdate(float delta);
    }

    public interface ICanUpdate
    {
        bool IsUpdate { get; set; }
    }

    public interface ICanFixedUpdate
    {
        bool IsFixedUpdate { get; set; }
    }

    public interface ICompose : ILifecycle, IUpdate, IFixedUpdate
    {
    }
}
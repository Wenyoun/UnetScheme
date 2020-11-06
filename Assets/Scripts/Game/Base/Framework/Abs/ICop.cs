namespace Zyq.Game.Base
{
    public interface ICop : ILifecycle
    {
        uint CopId { get; }

        IEntity Entity { set; }

        T CastEntity<T>() where T : IEntity;
    }
}
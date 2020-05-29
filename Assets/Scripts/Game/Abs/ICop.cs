using Base;

namespace Game
{
    public interface ICop : ILifecycle
    {
        IEntity Entity { get; set; }
    }
}
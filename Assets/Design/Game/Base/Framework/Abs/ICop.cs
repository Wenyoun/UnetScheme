using Zyq.Base;

namespace Zyq.Game.Base {
    public interface ICop : ILifecycle {
        IEntity Entity { get; set; }
    }
}
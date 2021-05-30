namespace Nice.Game.Base
{
    public interface ICop : ILifecycle
    {
        uint CopId { get; }

        Entity Entity { set; }
        
        AbsWorld World { set; }

        T CastEntity<T>() where T : Entity;
    }
}
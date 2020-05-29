namespace Game
{
    public interface IFeture
    {
        void OnInit(IEntity entity);

        void OnRemove();
    }

    public abstract class AbsFeture : IFeture
    {
        public virtual void OnInit(IEntity entity)
        {
        }

        public virtual void OnRemove()
        {
        }
    }
}
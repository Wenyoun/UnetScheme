namespace Nice.Game.Base
{
    public interface IWorldFeature
    {
        void OnInit(World world);

        void OnRemove();
    }

    public abstract class AbsWorldFeature : IWorldFeature
    {
        protected World m_World;

        public void OnInit(World world)
        {
            m_World = world;
            Init();
        }

        public void OnRemove()
        {
            Clear();
            m_World = null;
        }

        protected virtual void Init()
        {
        }

        protected virtual void Clear()
        {
        }
    }
}
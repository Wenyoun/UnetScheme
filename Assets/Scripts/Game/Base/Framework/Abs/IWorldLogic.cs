namespace Nice.Game.Base
{
    public interface IWorldFeature
    {
        void OnInit(AbsWorld world);

        void OnRemove();
    }

    public abstract class AbsWorldFeature : IWorldFeature
    {
        protected AbsWorld m_World;

        public void OnInit(AbsWorld world)
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
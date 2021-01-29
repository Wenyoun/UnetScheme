namespace Nice.Game.Base
{
    public interface IWorldLogic
    {
        void OnInit(IWorld world);

        void OnRemove();
    }

    public abstract class AbsWorldLogic : IWorldLogic
    {
        private IWorld m_World;

        public void OnInit(IWorld world)
        {
            m_World = world;
            Init();
        }

        public void OnRemove()
        {
            Clear();
            m_World = null;
        }

        protected IWorld World
        {
            get { return m_World; }
        }

        protected virtual void Init()
        {
        }

        protected virtual void Clear()
        {
        }

        protected void Register(int id, MsgDelegate handler)
        {
            m_World.Messager.Register(id, handler);
        }

        protected void UnRegister(int id, MsgDelegate handler)
        {
            m_World.Messager.UnRegister(id, handler);
        }
    }
}
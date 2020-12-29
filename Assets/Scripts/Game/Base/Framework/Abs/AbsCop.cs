namespace Nice.Game.Base
{
    public abstract class AbsCop : ICop
    {
        private uint m_CopId;
        private IEntity m_Entity;

        public AbsCop()
        {
            m_CopId = UniGenID.GenNextCopID();
        }

        public void OnInit()
        {
            Init();
        }

        public void OnRemove()
        {
            Clear();
        }

        public uint CopId
        {
            get { return m_CopId; }
        }

        public IEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        public T CastEntity<T>() where T : IEntity
        {
            return (T) m_Entity;
        }

        protected virtual void Init()
        {
        }

        protected virtual void Clear()
        {
        }
    }
}
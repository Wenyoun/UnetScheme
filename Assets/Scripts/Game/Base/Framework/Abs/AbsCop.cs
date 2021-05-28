namespace Nice.Game.Base
{
    public abstract class AbsCop : ICop
    {
        private uint m_CopId;
        private Entity m_Entity;

        protected AbsCop()
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
            m_Entity = null;
        }

        public uint CopId
        {
            get { return m_CopId; }
        }

        public Entity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        public T CastEntity<T>() where T : Entity
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
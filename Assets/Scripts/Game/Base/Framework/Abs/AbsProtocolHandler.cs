namespace Nice.Game.Base
{
    public abstract class AbsProtocolHandler
    {
        protected IConnection m_Connection;

        public IConnection Connection
        {
            set { m_Connection = value; }
        }

        public abstract void Register();

        public abstract void UnRegister();
    }
}
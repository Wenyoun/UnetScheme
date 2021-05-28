namespace Nice.Game.Server
{
    public interface IServerLogic
    {
        void OnInit(Server server);

        void OnRemove();
    }

    public abstract class AbsServerLogic : IServerLogic
    {
        protected Server m_Server;

        public void OnInit(Server server)
        {
            m_Server = server;
            Init();
        }

        public void OnRemove()
        {
            Clear();
            m_Server = null;
        }

        protected virtual void Init()
        {
        }

        protected virtual void Clear()
        {
        }
    }
}
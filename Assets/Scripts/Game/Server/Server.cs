using Nice.Game.Base;

namespace Nice.Game.Server
{
    public partial class Server : ICompose, IConnectionHandler
    {
        private UpdaterRegister m_UpdateManager;
        private ServerLogicManager m_LogicManager;
        private ServerWorldManager m_WorldManager;

        public Server()
        {
            m_UpdateManager = new UpdaterRegister();
            m_LogicManager = new ServerLogicManager();
            m_WorldManager = new ServerWorldManager();
        }

        public void OnInit()
        {
            NetworkServerManager.Bind(50000, this);
        }

        public void OnRemove()
        {
            m_WorldManager.Dispose();
            m_LogicManager.Dispose();
            m_UpdateManager.Dispose();
            NetworkServerManager.Dispose();
        }

        public void OnUpdate(float delta)
        {
            OnUpdater(delta);
            OnUpdateWorld(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            OnFixedUpdateWorld(delta);
        }

        public void OnLateUpdate(float delta)
        {
            OnLateUpdateWorld(delta);
        }

        public void OnAddConnection(IConnection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        public void OnRemoveConnection(IConnection connection)
        {
            connection.Disconnect();
        }
    }
}
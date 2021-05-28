using Nice.Game.Base;

namespace Nice.Game.Server
{
    public partial class Server : ICompose
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
            AddServerLogic<InitServerLogic>();
        }

        public void OnRemove()
        {
            m_WorldManager.Dispose();
            m_LogicManager.Dispose();
            m_UpdateManager.Dispose();
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
    }
}
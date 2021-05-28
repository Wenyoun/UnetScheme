using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class Server : ICompose, IConnectionHandler
    {
        private AbsWorld m_World;

        public Server()
        {
            m_World = new ServerWorld();
        }

        public void OnInit()
        {
            m_World.OnInit();
            NetworkServerManager.Bind(50000, this);
        }

        public void OnRemove()
        {
            NetworkServerManager.Dispose();
            m_World.Dispose();
        }

        public void OnUpdate(float delta)
        {
            m_World.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            m_World.OnFixedUpdate(delta);
        }

        public void OnLateUpdate(float delta)
        {
            m_World.OnLateUpdate(delta);
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
using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class Server : ICompose
    {
        #region single instance
        public static Server Ins;
        #endregion

        private World m_World;

        public Server()
        {
            Ins = this;
            NetworkServerManager.Init();
            m_World = new World();
        }

        public void OnInit()
        {
            NetworkServerManager.Bind(50000);
            m_World.OnInit();
        }

        public void OnRemove()
        {
            NetworkServerManager.Dispose();
            m_World.Dispose();
            Ins = null;
        }

        public void OnUpdate(float delta)
        {
            NetworkServerManager.OnUpdate();
            m_World.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            m_World.OnFixedUpdate(delta);
        }

        public void OnLateUpdate()
        {
            m_World.OnLateUpdate();
        }
    }
}
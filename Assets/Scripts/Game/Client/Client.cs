using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class Client : ICompose
    {
        #region single instance
        public static Client Ins;
        #endregion

        private World m_World;

        public Client()
        {
            Ins = this;
            m_World = new World();
        }

        public void OnInit()
        {
            m_World.OnInit();
            NetworkClient.Connect("127.0.0.1", 50000);
        }

        public void OnRemove()
        {
            m_World.Dispose();
            Ins = null;
        }

        public void OnUpdate(float delta)
        {
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

        public void Send(ushort cmd, ByteBuffer buffer)
        {
            m_World.Send(cmd, buffer);
        }

        public World World
        {
            get { return m_World; }
        }

        public Connection Connection
        {
            get { return m_World.Connection; }
        }
    }
}
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
            m_World = new World();
        }

        public void OnInit()
        {
            m_World.OnInit();
            Bind(50000);
        }

        public void OnRemove()
        {
            m_World.Dispose();
            Ins = null;
        }

        public void Bind(int port)
        {
            m_World.Bind(port);
        }

        public void Broadcast(ushort cmd, ByteBuffer buffer)
        {
            m_World.Broadcast(cmd, buffer);
        }

        public void Send(Connection connection, ushort cmd, ByteBuffer buffer)
        {
            m_World.Send(connection, cmd, buffer);
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
    }
}
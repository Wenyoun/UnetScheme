using Nice.Game.Base;

namespace Nice.Game.Client {
    public class Client : ICompose {
        #region single instance
        public static Client Ins;
        #endregion

        private ClientWorld m_World;

        public Client() {
            Ins = this;
            m_World = new ClientWorld();
        }

        public void OnInit() {
            m_World.OnInit();
            NetworkClientManager.Start("127.0.0.1", 50000, m_World);
        }

        public void OnRemove() {
            NetworkClientManager.Disconnect();
            m_World.Dispose();
            Ins = null;
        }

        public void OnUpdate(float delta) {
            m_World.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta) {
            m_World.OnFixedUpdate(delta);
        }

        public void OnLateUpdate(float delta) {
            m_World.OnLateUpdate(delta);
        }

        public World World {
            get { return m_World; }
        }
    }
}
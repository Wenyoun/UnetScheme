using Nice.Game.Base;

namespace Nice.Game.Client {
    public class Client : ICompose {
        private ClientWorld m_World;

        public Client() {
            m_World = new ClientWorld(1);
        }

        public void OnInit() {
            m_World.OnInit();
        }

        public void OnRemove() {
            m_World.Dispose();
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
    }
}
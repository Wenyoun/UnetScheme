using Nice.Game.Base;

namespace Nice.Game.Server
{
    public partial class Server
    {
        public void AddWorld(AbsWorld world)
        {
            m_WorldManager.AddWorld(world);
        }

        public void RemoveWorld(int wid)
        {
            m_WorldManager.RemoveWorld(wid);
        }

        private void OnUpdateWorld(float delta)
        {
            m_WorldManager.OnUpdate(delta);
        }

        private void OnFixedUpdateWorld(float delta)
        {
            m_WorldManager.OnFixedUpdate(delta);
        }

        private void OnLateUpdateWorld(float delta)
        {
            m_WorldManager.OnLateUpdate(delta);
        }
    }
}
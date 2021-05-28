using Nice.Game.Base;

namespace Nice.Game.Server
{
    public partial class Server
    {
        public void RegisterUpdate(UpdateDelegate handler)
        {
            m_UpdateManager.Register(handler);
        }

        public void UnRegisterUpdate(UpdateDelegate handler)
        {
            m_UpdateManager.UnRegister(handler);
        }

        private void OnUpdater(float delta)
        {
            m_UpdateManager.OnUpdate(delta);
        }
    }
}
namespace Nice.Game.Base
{
    public abstract partial class World
    {
        public void RegisterUpdate(UpdateDelegate handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Updater.Register(handler);
        }

        public void UnRegisterUpdate(UpdateDelegate handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Updater.UnRegister(handler);
        }

        public void RegisterFixedUpdate(UpdateDelegate handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_FixedUpdater.Register(handler);
        }

        public void UnRegisterFixedUpdate(UpdateDelegate handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_FixedUpdater.UnRegister(handler);
        }
    }
}
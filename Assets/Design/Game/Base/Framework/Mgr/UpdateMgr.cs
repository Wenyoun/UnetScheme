namespace Zyq.Game.Base
{
    public class UpdateMgr
    {
        private UpdateRegister m_Register = new UpdateRegister();

        public void Init()
        {
            m_Register.OnInit();
        }

        public void Dispose()
        {
            m_Register.Dispose();
        }

        public void OnUpdate(float delta)
        {
            m_Register.OnUpdate(delta);
        }

        public void OnLateUpdate()
        {
            m_Register.OnLateUpdate();
        }

        public void OnFixedUpdate(float delta)
        {
            m_Register.OnFixedUpdate(delta);
        }

        public void RegisterUpdate(UpdateDelegate update)
        {
            m_Register.RegisterUpdate(update);
        }

        public void UnregisterUpdate(UpdateDelegate update)
        {
            m_Register.UnregisterUpdate(update);
        }

        public void RegisterLateUpdate(LateUpdateDelegate update)
        {
            m_Register.RegisterLateUpdate(update);
        }

        public void UnregisterLateUpdate(LateUpdateDelegate update)
        {
            m_Register.UnregisterLateUpdate(update);
        }

        public void RegisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            m_Register.RegisterFixedUpdate(fixedUpdate);
        }

        public void UnregisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            m_Register.UnregisterFixedUpdate(fixedUpdate);
        }
    }
}
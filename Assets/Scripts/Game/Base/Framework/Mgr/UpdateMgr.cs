using System;

namespace Zyq.Game.Base
{
    public class UpdateMgr : IDisposable
    {
        private UpdateRegister m_Register;

        public UpdateMgr()
        {
            m_Register = new UpdateRegister();
        }

        public void Dispose()
        {
            m_Register.Dispose();
            m_Register = null;
        }

        public void OnUpdate(float delta)
        {
            if (m_Register != null)
            {
                m_Register.OnUpdate(delta);
            }
        }

        public void OnLateUpdate()
        {
            if (m_Register != null)
            {
                m_Register.OnLateUpdate();
            }
        }

        public void OnFixedUpdate(float delta)
        {
            if (m_Register != null)
            {
                m_Register.OnFixedUpdate(delta);
            }
        }

        public void RegisterUpdate(UpdateDelegate update)
        {
            if (m_Register != null)
            {
                m_Register.RegisterUpdate(update);
            }
        }

        public void UnregisterUpdate(UpdateDelegate update)
        {
            if (m_Register != null)
            {
                m_Register.UnregisterUpdate(update);
            }
        }

        public void RegisterLateUpdate(LateUpdateDelegate update)
        {
            if (m_Register != null)
            {
                m_Register.RegisterLateUpdate(update);
            }
        }

        public void UnregisterLateUpdate(LateUpdateDelegate update)
        {
            if (m_Register != null)
            {
                m_Register.UnregisterLateUpdate(update);
            }
        }

        public void RegisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (m_Register != null)
            {
                m_Register.RegisterFixedUpdate(fixedUpdate);
            }
        }

        public void UnregisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (m_Register != null)
            {
                m_Register.UnregisterFixedUpdate(fixedUpdate);
            }
        }
    }
}
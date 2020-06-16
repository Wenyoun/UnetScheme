using System;

namespace Zyq.Game.Base
{
    public class TimerMgr
    {
        private TimerRegister m_Register;

        public void Init()
        {
            m_Register = new TimerRegister();
        }

        public void Dispose()
        {
            m_Register.Dispose();
            m_Register = null;
        }

        public int Register(float delay, Action func)
        {
            return m_Register.Register(delay, delay, 1, func, null);
        }

        public void Unregister(int id)
        {
            m_Register.Unregister(id);
        }

        public void OnUpdate(float delta)
        {
            m_Register.OnUpdate(delta);
        }
    }
}
using System;

namespace Zyq.Game.Base
{
    public class TimerMgr : IDisposable
    {
        private TimerRegister m_Register;

        public TimerMgr()
        {
            m_Register = new TimerRegister();
        }

        public void Dispose()
        {
            if (m_Register != null)
            {
                m_Register.Dispose();
                m_Register = null;
            }
        }

        public int Register(float delay, Action func)
        {
            if (m_Register != null)
            {
                return m_Register.Register(delay, delay, 1, func, null);
            }

            return -1;
        }

        public int Register(float delay, float interval, int count, Action func, Action finish = null)
        {
            if (m_Register != null)
            {
                return m_Register.Register(delay, interval, count, func, finish);
            }

            return -1;
        }

        public int RegisterFrame(Action frame)
        {
            if (m_Register != null)
            {
                return m_Register.RegisterFrame(frame);
            }

            return -1;
        }

        public void Unregister(int id)
        {
            if (m_Register != null)
            {
                m_Register.Unregister(id);
            }
        }

        public void OnUpdate(float delta)
        {
            if (m_Register != null)
            {
                m_Register.OnUpdate(delta);
            }
        }
    }
}
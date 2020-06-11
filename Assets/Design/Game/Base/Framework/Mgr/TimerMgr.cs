using System;

namespace Zyq.Game.Base
{
    public class TimerMgr
    {
        private static TimerRegister m_Register;

        public static void Init()
        {
            m_Register = new TimerRegister();

            UpdateMgr.RegisterUpdate(OnUpdate);
        }

        public static void Dispose()
        {
            UpdateMgr.UnregisterUpdate(OnUpdate);

            m_Register.Dispose();
            m_Register = null;
        }

        public static int Register(float delay, Action func)
        {
            return m_Register.Register(delay, delay, 1, func, null);
        }

        public static void Unregister(int id)
        {
            m_Register.Unregister(id);
        }

        private static void OnUpdate(float delta)
        {
            m_Register.OnUpdate(delta);
        }
    }
}
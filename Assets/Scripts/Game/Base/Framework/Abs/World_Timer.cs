using System;

namespace Nice.Game.Base
{
    public abstract partial class World
    {
        public int RegisterTimer(float delay, Action func)
        {
            if (m_Dispose)
            {
                return -1;
            }
            return m_Timer.Register(delay, func);
        }

        public int RegisterTimer(float delay, float interval, int count, Action repeat, Action finish = null)
        {
            if (m_Dispose)
            {
                return -1;
            }
            return m_Timer.Register(delay, interval, count, repeat, finish);
        }

        public void UnRegisterTimer(int id)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Timer.UnRegister(id);
        }
    }
}
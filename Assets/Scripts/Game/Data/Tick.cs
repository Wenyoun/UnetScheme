namespace Game
{
    public class Tick
    {
        private float m_Time;
        private float m_Limit;

        public Tick(float limit)
        {
            m_Time = limit;
            m_Limit = limit;
        }

        public bool Ready(float delta)
        {
            m_Time = m_Time + delta;
            if (m_Time >= m_Limit)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            m_Time = 0;
        }
    }
}
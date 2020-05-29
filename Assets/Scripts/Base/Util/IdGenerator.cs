namespace Base
{
    public class IdGenerator
    {
        private int m_Value;

        public IdGenerator()
        {
            m_Value = 1;
        }

        public int Next { get { return m_Value++; } }
    }
}
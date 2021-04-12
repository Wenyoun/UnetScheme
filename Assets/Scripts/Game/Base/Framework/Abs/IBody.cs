using System;

namespace Nice.Game.Base
{
    public struct Body
    {
        private int m_Id;
        private Delegate m_Handler;
        private MessagerRegister m_Register;

        public Body(int id, Delegate handler, MessagerRegister register)
        {
            m_Id = id;
            m_Handler = handler;
            m_Register = register;
        }

        public void UnRegister()
        {
            m_Register.UnRegister(m_Id, m_Handler);
        }
    }
}
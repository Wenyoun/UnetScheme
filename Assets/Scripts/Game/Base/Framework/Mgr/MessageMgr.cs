namespace Zyq.Game.Base
{
    public class MessageMgr
    {
        private MsgRegister m_Register;

        public void Init()
        {
            m_Register = new MsgRegister();
        }

        public void Dispose()
        {
            m_Register.Dispose();
            m_Register = null;
        }

        public void Register(int id, MsgDelegate handler)
        {
            if (m_Register != null)
            {
                m_Register.Register(id, handler);
            }
        }

        public void Unregister(int id, MsgDelegate handler)
        {
            if (m_Register != null)
            {
                m_Register.Unregister(id, handler);
            }
        }

        public void Dispatcher(int id, IBody body = null)
        {
            if (m_Register != null)
            {
                m_Register.Dispatcher(id, body);
            }
        }
    }
}
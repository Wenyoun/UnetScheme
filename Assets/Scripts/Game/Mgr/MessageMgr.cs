namespace Game
{
    public class MessageMgr
    {
        private static MsgRegister m_Register;

        public static void Init()
        {
            m_Register = new MsgRegister();
        }

        public static void Dipose()
        {
            m_Register.Dispose();
            m_Register = null;
        }

        public static void Register(int id, MsgDelegate handler)
        {
            m_Register.Register(id, handler);
        }

        public static void Unregister(int id, MsgDelegate handler)
        {
            m_Register.Unregister(id, handler);
        }

        public static void Dispatcher(int id, IBody body = null)
        {
            m_Register.Dispatcher(id, body);
        }
    }
}
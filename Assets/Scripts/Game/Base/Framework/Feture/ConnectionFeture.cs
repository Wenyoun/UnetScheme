namespace Zyq.Game.Base
{
    public class ConnectionFeture : IFeture
    {
        #region Fields

        private Connection m_Connection;

        #endregion

        #region Properties

        public Connection Connection => m_Connection;

        #endregion

        public ConnectionFeture(Connection connection)
        {
            m_Connection = connection;
        }

        public void Send(ushort cmd, ByteBuffer buffer)
        {
            if (m_Connection != null)
            {
                m_Connection.Send(cmd, buffer);
            }
        }

        public void RegisterHandler(ushort cmd, ChannelMessageDelegate handler)
        {
            if (m_Connection != null)
            {
                m_Connection.RegisterHandler(cmd, handler);
            }
        }

        public void UnRegisterHandler(ushort id)
        {
            if (m_Connection != null)
            {
                m_Connection.UnRegisterHandler(id);
            }
        }
    }
}
namespace Nice.Game.Base
{
    public class ConnectionFeture : IFeture
    {
        private Connection m_Connection;
        public Connection Connection => m_Connection;

        public ConnectionFeture(Connection connection)
        {
            m_Connection = connection;
        }

        public void Send(ushort cmd, ByteBuffer buffer)
        {
            m_Connection.Send(cmd, buffer);
        }

        public void RegisterHandler(ushort cmd, ChannelMessageDelegate handler)
        {
            m_Connection.RegisterHandler(cmd, handler);
        }

        public void UnRegisterHandler(ushort id)
        {
            m_Connection.UnRegisterHandler(id);
        }
    }
}
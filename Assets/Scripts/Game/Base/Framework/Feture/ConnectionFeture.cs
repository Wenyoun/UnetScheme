using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public class ConnectionFeture : AbsFeture
    {
        private Connection m_Connection;
        public Connection Connection => m_Connection;

        public ConnectionFeture(Connection connection)
        {
            m_Connection = connection;
        }

        public void Send(NetworkWriter writer)
        {
            m_Connection.Send(writer);
        }

        public void RegisterHandler(short id, NetworkMessageDelegate handler)
        {
            m_Connection.RegisterHandler(id, handler);
        }

        public void UnregisterHandler(short id)
        {
            m_Connection.UnregisterHandler(id);
        }
    }
}
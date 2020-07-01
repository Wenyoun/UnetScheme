using UnityEngine.Networking;

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

        public void Send(NetworkWriter writer)
        {
            if (m_Connection != null)
            {
                m_Connection.Send(writer);
            }
        }

        public void RegisterHandler(short id, NetworkMessageDelegate handler)
        {
            if (m_Connection != null)
            {
                m_Connection.RegisterHandler(id, handler);
            }
        }

        public void UnregisterHandler(short id)
        {
            if (m_Connection != null)
            {
                m_Connection.UnregisterHandler(id);
            }
        }
    }
}
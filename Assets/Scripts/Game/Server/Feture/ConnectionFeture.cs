using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class ConnectionFeture : AbsFeture
    {
        public Connection m_Net;

        public ConnectionFeture(Connection net)
        {
            m_Net = net;
        }

        public Connection Net => m_Net;
    }
}
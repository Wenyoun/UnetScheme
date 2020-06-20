using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public class ConnectionFeture : AbsFeture
    {
        public Connection Connection { get; set; }

        public ConnectionFeture(Connection connection)
        {
            Connection = connection;
        }

        public void Send(NetworkWriter writer)
        {
            Connection.Send(writer);
        }
    }
}
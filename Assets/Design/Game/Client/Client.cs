using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class Client
    {
        public static Client Ins = new Client();

        public void Init()
        {
            ClientObjectRegisterHandler.Register();
        }

        public void Dispose()
        {
            Connection.Dispose();
            ClientObjectRegisterHandler.Unregister();

            Connection = null;
        }

        public void OnStartClient()
        {
        }

        public void OnStopClient()
        {
        }

        public void OnServerConnect(NetworkConnection net)
        {
            Connection = RegisterProtocols(new Connection(net));
        }

        public void OnServerDisconnect(NetworkConnection net)
        {
            Connection.ClearProtocols();
        }

        private Connection RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<ClientProtocolHandler>();
            return connection;
        }

        public Connection Connection { get; private set; }
    }
}
using UnityEngine.Networking;

using Zyq.Game.Base;

using System.Collections.Generic;

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
            Connection = new Connection(net);
            RegisterProtocols(Connection);
        }

        public void OnServerDisconnect(NetworkConnection net)
        {
            Connection.ClearProtocols();
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<ClientProtocolHandler>();
        }

        public Connection Connection { get; private set; }
    }
}
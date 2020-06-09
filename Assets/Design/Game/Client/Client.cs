using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public class Client {
        public static Client Ins = new Client();

        public void Init() { }

        public void Dispose() {
            Connection.Dispose();
            Connection = null;
        }

        public void OnStartClient() { }

        public void OnStopClient() { }

        public void OnServerConnect(NetworkConnection net) {
            Connection = RegisterProtocols(new Connection(net));
            SendServer.LoginReq(Connection, "yinhuayo", "hong");
        }

        public void OnServerDisconnect(NetworkConnection net) {
            Connection.ClearProtocols();
        }

        private Connection RegisterProtocols(Connection connection) {
            connection.RegisterProtocol<ClientProtocolHandler>();
            return connection;
        }

        public Connection Connection { get; private set; }
    }
}
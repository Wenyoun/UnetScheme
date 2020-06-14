using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class Client
    {
        public static Client Ins = new Client();

        public void Init() { }

        public void Dispose()
        {
            Connection.Dispose();
            Connection = null;
        }

        public void OnStartClient() { }

        public void OnStopClient() { }

        public void OnServerConnect(NetworkConnection net)
        {

            Connection = RegisterProtocols(new Connection(net));
            Sender.Login(Client.Ins.Connection, 1, true, 2, 3, 4, 5, 6, 7, 8, 9, "yinhuayong", Vector2.zero, Vector3.zero, Vector4.zero, Quaternion.identity);
        }

        public void OnServerDisconnect(NetworkConnection net)
        {
            Connection.ClearRegisterProtocols();
        }

        private Connection RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
            return connection;
        }

        public Connection Connection { get; private set; }
    }
}
using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class Client : AbsMachine
    {
        public static Client Ins = new Client();
        public Connection Connection { get; private set; }

        private Client()
        {
        }

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            Connection.Dispose();
            Connection = null;
        }

        public void OnStartClient()
        {
        }

        public void OnStopClient()
        {
        }

        public override void OnNetConnect(NetworkConnection net)
        {
            Connection = new Connection(net);
            RegisterProtocols(Connection);
            Sender.RpcLogin(Connection, 1, true, 2, 3, 4, 5, 6, 7, 8, 9, "yinhuayong", Vector2.zero, Vector3.zero, Vector4.zero, Quaternion.identity);
        }

        public override void OnNetDisconnect(NetworkConnection net)
        {
            Connection.Dispose();
            Connection = null;
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
        }
    }
}
using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class Client : AbsMachine
    {
        public static Client Ins = new Client();
        public Connection Connection { get; private set; }
        public ClientEntityMgr EntityMgr { get; private set; }

        private Client()
        {
        }

        public override void OnInit()
        {
            base.OnInit();
            EntityMgr = new ClientEntityMgr();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            EntityMgr.Dispose();
            EntityMgr = null;
            Connection.Dispose();
            Connection = null;
        }

        public void OnStartClient()
        {
        }

        public void OnStopClient()
        {
        }

        public void Send(NetworkWriter writer)
        {
            if (Connection != null)
            {
                Connection.Send(writer);
            }
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            EntityMgr.OnUpdate(delta);
        }

        public override void OnFixedUpdate(float delta)
        {
            base.OnFixedUpdate(delta);
            EntityMgr.OnFixedUpdate(delta);
        }

        public override void OnNetConnect(NetworkConnection network)
        {
            if (Connection == null)
            {
                Connection = new Connection();
            }
            Connection.OnConnect(network);
            RegisterProtocols(Connection);
            Sender.RpcLogin(1, true, 2, 3, 4, 5, 6, 7, 8, 9, "yinhuayong", Vector2.zero, Vector3.zero, Vector4.zero, Quaternion.identity);
        }

        public override void OnNetDisconnect(NetworkConnection network)
        {
            Connection.OnDisconnect(network);
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
        }
    }
}
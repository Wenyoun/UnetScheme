using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class Client : IUpdate, IFixedUpdate
    {
        public static Client Ins = new Client();
        public EntityMgr EntityMgr { get; private set; }
        public UpdateMgr UpdateMgr { get; private set; }
        public MessageMgr MessageMgr { get; private set; }
        public TimerMgr TimerMgr { get; private set; }

        private Client()
        {
            EntityMgr = new EntityMgr();
            UpdateMgr = new UpdateMgr();
            MessageMgr = new MessageMgr();
            TimerMgr = new TimerMgr();
        }

        public void Init()
        {
            UpdateMgr.Init();
            MessageMgr.Init();
            TimerMgr.Init();
            EntityMgr.Init();
        }

        public void Dispose()
        {
            EntityMgr.Dispose();
            TimerMgr.Dispose();
            MessageMgr.Dispose();
            UpdateMgr.Dispose();
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

        public void OnUpdate(float delta)
        {
            TimerMgr.OnUpdate(delta);
            UpdateMgr.OnUpdate(delta);
            EntityMgr.OnUpdate(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            UpdateMgr.OnFixedUpdate(delta);
            EntityMgr.OnFixedUpdate(delta);
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
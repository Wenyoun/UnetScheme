using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Game.Host
{
    public class HostNetworkManager : NetworkManager
    {
        public static HostNetworkManager Ins;

        private void Awake()
        {
            Ins = this;

            Server.Server.Ins.OnInit();
            Client.Client.Ins.OnInit();
        }

        private void OnDestroy()
        {
            Ins = null;

            Server.Server.Ins.OnRemove();
            Client.Client.Ins.OnRemove();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Client.Client.Ins.OnStartClient();
            Server.Server.Ins.OnStartServer();

            Debug.Log("OnStartServer");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            Client.Client.Ins.OnStopClient();
            Server.Server.Ins.OnStopServer();

            Debug.Log("OnStopServer");
        }

        public override void OnServerConnect(NetworkConnection net)
        {
            base.OnServerConnect(net);

            Debug.Log("OnServerConnect");

            Server.Server.Ins.OnClientConnect(net);
        }

        public override void OnServerDisconnect(NetworkConnection net)
        {
            base.OnServerDisconnect(net);

            Server.Server.Ins.OnClientDisconnect(net);

            Debug.Log("OnServerDisconnect");
        }

        public override void OnClientConnect(NetworkConnection net)
        {
            base.OnClientConnect(net);

            Debug.Log("OnClientConnect");

            Client.Client.Ins.OnServerConnect(net);
        }

        public override void OnClientDisconnect(NetworkConnection net)
        {
            base.OnClientDisconnect(net);

            Client.Client.Ins.OnServerDisconnect(net);

            Debug.Log("OnClientDisconnect");
        }

        private void Update()
        {
            Client.Client.Ins.OnUpdate(Time.deltaTime);
            Server.Server.Ins.OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Client.Client.Ins.OnFixedUpdate(Time.deltaTime);
            Server.Server.Ins.OnFixedUpdate(Time.deltaTime);
        }
    }
}
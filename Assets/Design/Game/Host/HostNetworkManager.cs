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

            Server.Server.Ins.Init();
            Client.Client.Ins.Init();
        }

        private void OnDestroy()
        {
            Ins = null;

            Server.Server.Ins.Dispose();
            Client.Client.Ins.Dispose();
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

            Server.Server.Ins.OnClientConnect(net);

            Debug.Log("OnServerConnect");
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

            Client.Client.Ins.OnServerConnect(net);

            Debug.Log("OnClientConnect");
        }

        public override void OnClientDisconnect(NetworkConnection net)
        {
            base.OnClientDisconnect(net);

            Client.Client.Ins.OnServerDisconnect(net);

            Debug.Log("OnClientDisconnect");
        }
    }
}
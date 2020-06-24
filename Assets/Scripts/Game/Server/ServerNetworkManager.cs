using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class ServerNetworkManager : NetworkManager
    {
        private void Awake()
        {
            Server.Ins.OnInit();
        }

        private void OnDestroy()
        {
            Server.Ins.OnRemove();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Server.Ins.OnStartServer();

            Debug.Log("OnStartServer");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            Server.Ins.OnStopServer();

            Debug.Log("OnStopServer");
        }

        public override void OnServerConnect(NetworkConnection net)
        {
            base.OnServerConnect(net);

            Server.Ins.OnNetConnect(net);

            Debug.Log("OnServerConnect");
        }

        public override void OnServerDisconnect(NetworkConnection net)
        {
            base.OnServerDisconnect(net);

            Server.Ins.OnNetDisconnect(net);

            Debug.Log("OnServerDisconnect");
        }

        private void Update()
        {
            Server.Ins.OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Server.Ins.OnFixedUpdate(Time.deltaTime);
        }
    }
}
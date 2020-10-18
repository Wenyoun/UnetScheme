using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Game.Host
{
    public class HostNetworkManager : NetworkManager
    {
        private void Awake()
        {
            Server.Server.Ins.OnInit();
            Client.Client.Ins.OnInit();
        }

        private void OnDestroy()
        {
            Server.Server.Ins.OnRemove();
            Client.Client.Ins.OnRemove();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Server.Server.Ins.OnStartServer();
            Client.Client.Ins.OnStartClient();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            Server.Server.Ins.OnStopServer();
            Client.Client.Ins.OnStopClient();
        }

        private void Update()
        {
            Server.Server.Ins.OnUpdate(Time.deltaTime);
            Client.Client.Ins.OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Server.Server.Ins.OnFixedUpdate(Time.deltaTime);
            Client.Client.Ins.OnFixedUpdate(Time.deltaTime);
        }
    }
}
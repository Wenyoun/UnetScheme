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
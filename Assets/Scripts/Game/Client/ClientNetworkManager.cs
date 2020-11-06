using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class ClientNetworkManager : NetworkManager
    {
        private void Awake()
        {
            Client.Ins.OnInit();
        }

        private void OnDestroy()
        {
            Client.Ins.OnRemove();
        }

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);

            Debug.Log("OnStartClient");
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            Debug.Log("OnStopClient");
        }

        private void Update()
        {
            Client.Ins.OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Client.Ins.OnFixedUpdate(Time.deltaTime);
        }
    }
}
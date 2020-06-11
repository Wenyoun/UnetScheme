using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Game.Client {
    public class ClientNetworkManager : NetworkManager {
        public static ClientNetworkManager Ins;

        private void Awake() {
            Ins = this;

            Client.Ins.Init();
        }

        private void OnDestroy() {
            Ins = null;

            Client.Ins.Dispose();
        }

        public override void OnStartClient(NetworkClient client) {
            base.OnStartClient(client);

            Client.Ins.OnStartClient();

            Debug.Log("OnStartClient");
        }

        public override void OnStopClient() {
            base.OnStopClient();

            Client.Ins.OnStopClient();

            Debug.Log("OnStopClient");
        }

        public override void OnClientConnect(NetworkConnection net) {
            base.OnClientConnect(net);

            Client.Ins.OnServerConnect(net);

            Debug.Log("OnClientConnect");
        }

        public override void OnClientDisconnect(NetworkConnection net) {
            base.OnClientDisconnect(net);

            Client.Ins.OnServerDisconnect(net);

            Debug.Log("OnClientDisconnect");
        }
    }
}
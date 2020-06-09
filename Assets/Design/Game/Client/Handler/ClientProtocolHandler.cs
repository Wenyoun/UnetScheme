using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public class ClientProtocolHandler : IProtocolHandler {
        public Connection Connection { get; set; }

        public void Register() {
            SendServer.Login(Connection, "jfljalfjlajka", 10);
        }

        public void Unregister() { }
    }
}
using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Game.Server {
    public class RecvClient {
        [Recv(MsgId.Msg_Login_Req)]
        public static void OnLogin(Connection connection, string k1, float k2) {
            Debug.Log("resul1t:" + k1 + ":::" + k2);
        }
    }
}
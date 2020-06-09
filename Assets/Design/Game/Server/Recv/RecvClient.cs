using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server {
    public class RecvClient {
        [Recv(MsgId.Msg_Login_Req)]
        public static void OnLogin(Connection connection, string username, string password) {
            Debug.Log("服务器收到客户端的登陆消息:" + username + ":::" + password);
            SendClient.OnLoginSuccess(connection, username, password, 1);
            SendClient.OnCreatePlayeer(connection, username, password, 100000, 500000);
        }
    }
}
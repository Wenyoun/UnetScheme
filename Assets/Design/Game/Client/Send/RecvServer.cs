using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public static class RecvServer {
        [Recv(MsgId.Msg_Login_Res)]
        public static void OnLoginRsp(Connection connection, string username, string password, int result) {
            Debug.Log("客户端收到服务器的返回消息:" + username + ":::" + password + ":::" + result);
        }

        [Recv(MsgId.Msg_Create_Player)]
        public static void OnCreatePlayerRsp(Connection connection, string username, string password, int k, float b) {
            Debug.Log("客户端收到服务器创建玩家的消息:" + username + ":::" + password + ":::" + k + ":::" + b);
        }
    }
}
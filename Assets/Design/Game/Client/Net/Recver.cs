using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public static class Recver {
        [Recv(MsgId.Msg_Login_Res)]
        public static void OnLoginRsp(Connection connection, string username, string password, bool result) {
            Debug.Log("客户端收到服务器的登陆结果:" + username + ":::" + password + ":::" + (result ? "登陆成功" : "登陆失败"));
        }
    }
}
using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public static class Recver
    {
        [Recv(NetMsgId.Msg_Login_Res)]
        public static void OnLoginRsp(Connection connection, bool result)
        {
            Debug.Log("客户端收到服务器的登陆结果:" + (result ? "登录成功" : "登录失败1"));
        }

        [Recv(NetMsgId.Msg_Create_Player)]
        public static void OnCreatePlayer(Connection connection, uint eid, uint gid, string username)
        {
            Debug.Log("客户端创建Entity:" + eid + "," + gid + "," + username);
            Entity entity = EntityFactory.CreatePlayer(connection, eid, gid);
            Client.Ins.EntityMgr.AddEntity(entity);
        }

        [Recv(NetMsgId.Msg_Create_Player1)]
        public static void OnCreatePlayer1(Connection connection, uint eid, string username)
        {
            Debug.Log("11111:::");
        }
    }
}
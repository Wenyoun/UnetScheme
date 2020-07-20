using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Client
{
    public static class ClientRecver
    {
        [Recv(NetMsgId.Msg_Login_Res)]
        public static void OnLoginRsp(bool result)
        {
            Debug.Log("客户端收到服务器的登陆结果:" + (result ? "登录成功" : "登录失败"));
        }

        [Recv(NetMsgId.Msg_Create_Player)]
        public static void OnCreatePlayer(uint eid, uint gid, Vector3 position)
        {
            Debug.Log("客户端创建Entity:" + eid + "," + gid + "," + position);
            Entity entity = EntityFactory.CreatePlayer(eid, gid, position);
            Client.Ins.EntityMgr.AddEntity(entity);
        }

        [Recv(NetMsgId.Msg_Create_Player2)]
        public static void OnRecvArray(int v1, int[] v2, LoginData[] v4)
        {
        }
    }
}
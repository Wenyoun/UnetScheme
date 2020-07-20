using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Server
{
    public class ServerSender
    {
        [Send(NetMsgId.Msg_Login_Res)]
        public static void RpcTargetLoginResult(Connection connection, bool result)
        {
        }

        [Broadcast(NetMsgId.Msg_Create_Player)]
        public static void BroadcastCreatePlayer(uint eid, uint gid, Vector3 position)
        {
        }

        [Send(NetMsgId.Msg_Create_Player2)]
        public static void _____RpcArray(Connection connection, int v1, int[] v2, string[] v3, Login[] v4, Vector3[] v5, LoginData[] v6)
        {
        }

        [Broadcast(NetMsgId.Msg_Create_Player2)]
        public static void _____RpcArray____(Connection connection, int v1, int[] v2, string[] v3, Login[] v4, Vector3[] v5, LoginData[] v6)
        {
        }
    }
}
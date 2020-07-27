using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Server
{
    public class ServerSender
    {
        [Send(NetMsgId.Msg_Login_Res)]
        public static void RpcTargetLoginResult(Connection connection, bool result, LoginData data)
        {
        }

        [Broadcast(NetMsgId.Msg_Create_Player)]
        public static void BroadcastCreatePlayer(uint eid, uint gid, Vector3 position)
        {
        }
    }
}
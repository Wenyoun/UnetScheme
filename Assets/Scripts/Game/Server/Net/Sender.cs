using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class Sender
    {
        [Send(NetMsgId.Msg_Login_Res)]
        public static void RpcTargetLoginResult(Connection connection, bool result)
        {
        }

        [Broadcast(NetMsgId.Msg_Create_Player)]
        public static void BroadcastCreatePlayer(Connection connection, uint eid, uint gid, string username)
        {
        }
    }
}
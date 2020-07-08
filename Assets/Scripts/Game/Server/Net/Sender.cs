using UnityEngine;
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
        public static void BroadcastCreatePlayer(uint eid, uint gid, Vector3 position)
        {
        }
    }

    public struct Test1
    {
        public int v1;
        public double v2;
        public Test2 v3;
        public string v4;
    }

    public struct Test2
    {
        public int v1;
        public double v2;
        public string v3;
    }
}
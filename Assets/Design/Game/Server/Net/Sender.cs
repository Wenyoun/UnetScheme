using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class Sender
    {
        [Send(MsgId.Msg_Login_Res)]
        public static void LoginResult(Connection connection, bool result) { }

        [Send(MsgId.Msg_Create_Player)]
        public static void CreatePlayer(Connection connection, uint eid, uint gid, string username) { }
    }
}
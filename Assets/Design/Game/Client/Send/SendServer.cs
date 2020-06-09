using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public static class SendServer {
        [Send(MsgId.Msg_Login_Req)]
        public static void LoginReq(Connection connection, string username, string password) { }
    }
}
using Zyq.Game.Base;

namespace Zyq.Game.Server {
    public class Sender {
        [Send(MsgId.Msg_Login_Res)]
        public static void LoginResult(Connection connection, string username, string password, bool result) { }
    }
}
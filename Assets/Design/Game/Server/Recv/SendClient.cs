using Zyq.Game.Base;

namespace Zyq.Game.Server {
    public class SendClient {
        [Send(MsgId.Msg_Login_Res)]
        public static void OnLoginSuccess(Connection connection, string username, string password, int result) { }

        [Send(MsgId.Msg_Create_Player)]
        public static void OnCreatePlayeer(Connection connection, string username, string password, int k, float b) { }
    }
}
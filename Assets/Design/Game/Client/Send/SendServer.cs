using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public static class SendServer {
        [Send(MsgId.Msg_Login_Req)]
        public static void Login(Connection connection, string k1, float k2) {
            /**
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(MsgId.Msg_Login_Req);
            writer.Write(k1);
            connection.Send(writer);
            **/
        }
    }
}
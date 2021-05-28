using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class Recver
    {
        [Recv(MsgID.Login)]
        public static void Login(IConnection connection, SLogin login, ByteBuffer buffer, byte[] data, int k)
        {
            Debug.Log(login.Token + "," + login.PlayerID + "," + login.Timestamp + "," + login.Flag + "," + k + "," + buffer.ReadString() + "," + k);
            LoginRsp(connection, login);
        }

        [Send(MsgID.LoginRsp)]
        public static void LoginRsp(IConnection connection, SLogin login)
        {
        }
    }
}
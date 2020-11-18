using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class Recver
    {
        [Recv(MsgID.Login)]
        public static void Login(Connection connection, SLogin login)
        {
            Debug.Log(login.Token + "," + login.PlayerID + "," + login.Timestamp + "," + login.Flag);
            LoginRsp(connection, login);
        }

        [Send(MsgID.LoginRsp)]
        public static void LoginRsp(Connection connection, SLogin login)
        {
        }
    }
}
using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class Sender
    {
        [Send(MsgID.Login)]
        public static void Login(SLogin login)
        {
        }

        [Recv(MsgID.LoginRsp)]
        public static void LoginRsp(SLogin login)
        {
            Debug.Log(login.PlayerID);
        }
    }
}
using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
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
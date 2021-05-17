using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Client
{
    public static class Sender
    {
        [Send(MsgID.Login, ChannelType.Unreliable)]
        public static void Login(SLogin login, ByteBuffer buffer, byte[] data, int k)
        {
        }

        [Recv(MsgID.LoginRsp)]
        public static void LoginRsp(SLogin login)
        {
            Debug.Log(login.PlayerID);
        }
    }
}
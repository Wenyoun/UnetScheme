using System.Text;
using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class ConnectLogic : AbsWorldLogic
    {
        protected override void Init()
        {
            World.Messager.Register(MessageConstants.Connect_Success, OnConnectSuccess);
            World.Messager.Register(MessageConstants.Connect_Error, OnConnectError);
        }

        protected override void Clear()
        {
            World.Messager.UnRegister(MessageConstants.Connect_Success, OnConnectSuccess);
            World.Messager.UnRegister(MessageConstants.Connect_Error, OnConnectError);
        }

        private void OnConnectSuccess(IBody body)
        {
            SLogin login = new SLogin();
            login.PlayerID = 567;
            login.Token = "yinhuayong";
            login.Timestamp = 10;
            login.Flag = 20;
            ByteBuffer buffer = ByteBuffer.Allocate(512);
            buffer.Write("零下206度5");
            Sender.Login(login, buffer, Encoding.UTF8.GetBytes("11"), 567);
        }

        private void OnConnectError(IBody body)
        {
        }
    }
}
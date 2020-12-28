using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
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
            Sender.Login(login);
        }

        private void OnConnectError(IBody body)
        {
        }
    }
}
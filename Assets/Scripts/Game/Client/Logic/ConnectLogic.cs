using System.Text;
using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class ConnectLogic : AbsWorldLogic
    {
        protected override void Init()
        {
            Register(MessageConstants.Connect_Success, OnConnectSuccess);
            Register(MessageConstants.Connect_Error, OnConnectError);
        }

        protected override void Clear()
        {
            UnRegister(MessageConstants.Connect_Success, OnConnectSuccess);
            UnRegister(MessageConstants.Connect_Error, OnConnectError);
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
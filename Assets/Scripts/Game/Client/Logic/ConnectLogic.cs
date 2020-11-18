using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class ConnectLogic : IWorldLogic
    {
        private World m_World;

        public void OnInit(IWorld world)
        {
            m_World = (World) world;
            m_World.Message.Register(MessageConstants.Connect_Success, OnConnectSuccess);
            m_World.Message.Register(MessageConstants.Connect_Error, OnConnectError);
        }

        public void OnRemove()
        {
            m_World.Message.Unregister(MessageConstants.Connect_Success, OnConnectSuccess);
            m_World.Message.Unregister(MessageConstants.Connect_Error, OnConnectError);
            m_World = null;
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
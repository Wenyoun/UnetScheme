using System.Text;
using Nice.Game.Base;

namespace Nice.Game.Client {
    public class ConnectFeature : AbsWorldFeature {
        protected override void Init() {
            m_World.RegisterMessage<IConnection>(MessageConstants.AddConnection, OnAddConnection);
            m_World.RegisterMessage<IConnection>(MessageConstants.RemoveConnection, OnRemoveConnection);
        }

        protected override void Clear() {
            m_World.UnRegisterMessage<IConnection>(MessageConstants.AddConnection, OnAddConnection);
            m_World.UnRegisterMessage<IConnection>(MessageConstants.RemoveConnection, OnRemoveConnection);
        }

        private void OnAddConnection(Body body, IConnection connection) {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>().SetWorld(m_World);

            SLogin login = new SLogin();
            login.PlayerID = 567;
            login.Token = "yinhuayong";
            login.Timestamp = 10;
            login.Flag = 20;
            ByteBuffer buffer = ByteBuffer.Allocate(512);
            buffer.Write("零下206度5");
            Sender.Login(login, buffer, Encoding.UTF8.GetBytes("11"), 1000);
        }

        private void OnRemoveConnection(Body body, IConnection connection) {
            connection.Disconnect();
        }
    }
}
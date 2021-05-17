using System.Text;
using Nice.Game.Base;

namespace Nice.Game.Client {
    public class ConnectFeature : AbsWorldFeature {
        protected override void Init() {
            m_World.RegisterMessage<Connection>(MessageConstants.Connect, OnConnect);
            m_World.RegisterMessage<Connection>(MessageConstants.Disconnect, OnDisconnect);
        }

        protected override void Clear() {
            m_World.UnRegisterMessage<Connection>(MessageConstants.Connect, OnConnect);
            m_World.UnRegisterMessage<Connection>(MessageConstants.Disconnect, OnDisconnect);
        }

        private void OnConnect(Body body, Connection connection) {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();

            SLogin login = new SLogin();
            login.PlayerID = 567;
            login.Token = "yinhuayong";
            login.Timestamp = 10;
            login.Flag = 20;
            ByteBuffer buffer = ByteBuffer.Allocate(512);
            buffer.Write("零下206度5");
            Sender.Login(login, buffer, Encoding.UTF8.GetBytes("11"), 567);
        }

        private void OnDisconnect(Body body, Connection connection) {
            connection.Dispose();
        }
    }
}
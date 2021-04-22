using System.Net;
using System.Net.Sockets;

namespace Nice.Game.Base {
    public sealed class KcpConClient : KcpConn {
        public KcpConClient(uint conId, uint conv, Socket socket) : base(conId, conv, socket, null) {
        }

        protected override void OnSendData(byte[] data, int offset, int size) {
            m_Socket.Send(data, offset, size, SocketFlags.None);
        }
    }
}
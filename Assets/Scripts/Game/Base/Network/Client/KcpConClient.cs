using System.Net.Sockets;

namespace Nice.Game.Base
{
    internal sealed class KcpConClient : KcpCon
    {
        public KcpConClient(uint conId, uint conv, Socket socket) : base(conId, conv, socket, null)
        {
        }

        protected override void OnSendData(byte[] data, int offset, int size)
        {
            m_Socket.Send(data, offset, size, SocketFlags.None);
        }
    }
}
using System.Net;
using System.Net.Sockets;

namespace Nice.Game.Base
{
    public sealed class KcpConServer : KcpCon
    {
        public KcpConServer(uint conId, uint conv, Socket socket, EndPoint point) : base(conId, conv, socket, point)
        {
        }

        protected override void OnSendData(byte[] data, int offset, int size)
        {
            m_Socket.SendTo(data, offset, size, SocketFlags.None, m_Point);
        }
    }
}
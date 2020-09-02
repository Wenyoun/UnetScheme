using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Net.KcpImpl;

namespace Base.Net.Impl
{
    public class KcpConn : IKcpCallback, IDisposable
    {
        private Kcp kcp;
        private long conId;
        private Socket udp;
        private EndPoint point;
        private byte[] outputBuffer;

        public KcpConn(uint conv, Socket udp)
        {
            this.conId = 1;
            this.udp = udp;
            this.point = null;
            this.kcp = new Kcp(conv, this);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.outputBuffer = new byte[2048];
        }

        public KcpConn(long conId, uint conv, Socket udp, EndPoint point)
        {
            this.udp = udp;
            this.conId = conId;
            this.point = point;
            this.kcp = new Kcp(conv, this);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.outputBuffer = new byte[2048];
        }

        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }

        public void Output(IMemoryOwner<byte> memory, int length)
        {
            if (udp != null)
            {
                memory.Memory.Span.Slice(0, length).CopyTo(new Span<byte>(outputBuffer, 0, outputBuffer.Length));
                if (point == null)
                {
                    udp.Send(outputBuffer, length, SocketFlags.None);
                }
                else
                {
                    udp.SendTo(outputBuffer, length, SocketFlags.None, point);
                }
            }
        }

        public int Send(byte[] buffer, int offset, int length)
        {
            if (buffer != null && kcp != null)
            {
                return kcp.Send(new Span<byte>(buffer, offset, length));
            }

            return -10;
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            if (buffer != null && kcp != null)
            {
                return kcp.Recv(new Span<byte>(buffer, offset, length));
            }

            return -10;
        }

        internal void Update(DateTime now)
        {
            if (udp != null && kcp != null)
            {
                kcp.Update(now);
            }
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (kcp != null)
            {
                kcp.Input(new Span<byte>(buffer, offset, length));
            }
        }

        public void Dispose()
        {
            conId = 0;
            if (kcp != null)
            {
                kcp.Dispose();
                kcp = null;
            }

            udp = null;
            point = null;
            outputBuffer = null;
        }
    }
}
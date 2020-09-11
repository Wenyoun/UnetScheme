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
        private uint conv;
        private long conId;
        private Socket udp;
        private EndPoint point;
        private bool isDispose;
        private bool isConnected;
        private byte[] outputBuffer;

        public KcpConn(uint conv, Socket udp)
        {
            this.conId = conv;
            this.udp = udp;
            this.conv = conv;
            this.point = null;
            this.kcp = new Kcp(conv, this);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.isDispose = false;
            this.isConnected = false;
            this.outputBuffer = new byte[1500];
        }

        public KcpConn(long conId, uint conv, Socket udp, EndPoint point)
        {
            this.udp = udp;
            this.conv = conv;
            this.conId = conId;
            this.point = point;
            this.kcp = new Kcp(conv, this);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.isDispose = false;
            this.isConnected = false;
            this.outputBuffer = new byte[1500];
        }

        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }

        public void Output(IMemoryOwner<byte> memory, int length)
        {
            if (!isDispose && udp != null)
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

        public int PeekSize()
        {
            if (!isDispose && kcp != null)
            {
                return kcp.PeekSize();
            }

            return -1;
        }

        public int Send(byte[] buffer, int offset, int length)
        {
            if (!isDispose && buffer != null && kcp != null)
            {
                return kcp.Send(new Span<byte>(buffer, offset, length));
            }

            return -10;
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            if (!isDispose && buffer != null && kcp != null)
            {
                return kcp.Recv(new Span<byte>(buffer, offset, length));
            }

            return -10;
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;
                
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

        #region internal
        internal void Update(DateTime now)
        {
            if (!isDispose && udp != null && kcp != null)
            {
                kcp.Update(now);
            }
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (!isDispose && kcp != null)
            {
                kcp.Input(new Span<byte>(buffer, offset, length));
            }
        }
        #endregion

        #region properties 
        public uint Conv => conv;
        
        public long ConId => conId;

        public bool IsConnected
        {
            get => isConnected;
            internal set => isConnected = value;
        }
        #endregion
    }
}
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Net.KcpImpl;

namespace Zyq.Game.Base
{
    public class KcpConn : IKcpCallback, IRentable, IDisposable
    {
        public const int HEAD_SIZE = 8;

        #region pool
        private class MemoryPool : IMemoryOwner<byte>
        {
            private Memory<byte> memory;

            public MemoryPool(int size)
            {
                memory = new Memory<byte>(new byte[size]);
            }

            public Memory<byte> Memory
            {
                get { return memory; }
            }

            public void Dispose()
            {
                memory = null;
            }
        }
        #endregion

        private Kcp m_Kcp;
        private Socket m_Socket;
        private EndPoint m_Point;

        private uint m_Conv;
        private long m_ConId;

        private bool m_Dispose;
        private bool m_IsConnected;
        private byte[] m_OutputBuffer;

        public KcpConn(long conId, uint conv, Socket socket) : this(conId, conv, socket, null)
        {
        }

        public KcpConn(long conId, uint conv, Socket socket, EndPoint point)
        {
            m_Point = point;
            m_Socket = socket;
            m_Kcp = new Kcp(conv, this, this);
            m_Kcp.NoDelay(1, 10, 2, 1);

            m_Conv = conv;
            m_ConId = conId;

            m_Dispose = false;
            m_IsConnected = false;
            m_OutputBuffer = new byte[KcpConstants.Packet_Length];
        }

        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return new MemoryPool(length);
        }

        public void Output(IMemoryOwner<byte> owner, int length)
        {
            if (m_Dispose)
            {
                return;
            }

            Memory<byte> memory = owner.Memory;
            KcpHelper.Encode64(m_OutputBuffer, 0, m_ConId);
            memory.Span.Slice(0, length).CopyTo(new Span<byte>(m_OutputBuffer, HEAD_SIZE, m_OutputBuffer.Length - HEAD_SIZE));

            if (m_Point == null)
            {
                m_Socket.Send(m_OutputBuffer, length + HEAD_SIZE, SocketFlags.None);
            }
            else
            {
                m_Socket.SendTo(m_OutputBuffer, length + HEAD_SIZE, SocketFlags.None, m_Point);
            }
        }

        public int Send(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -10;
            }
            return m_Kcp.Send(new Span<byte>(buffer, offset, length));
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -10;
            }
            return m_Kcp.Recv(new Span<byte>(buffer, offset, length));
        }

        public void Dispose()
        {
            lock (this)
            {
                if (m_Dispose)
                {
                    return;
                }

                m_Dispose = true;
            }

            m_IsConnected = false;
            m_Kcp.Dispose();

            m_Kcp = null;
            m_Socket = null;
            m_Point = null;
            m_OutputBuffer = null;
        }

        #region internal
        internal void Flush()
        {
            if (m_Dispose)
            {
                return;
            }

            m_Kcp.Flush();
        }

        internal void Update(long time)
        {
            if (m_Dispose)
            {
                return;
            }

            m_Kcp.Update(time);
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return;
            }

            m_Kcp.Input(new Span<byte>(buffer, offset, length));
        }
        #endregion

        #region properties
        public uint Conv
        {
            get { return m_Conv; }
        }

        public long ConId
        {
            get { return m_ConId; }
        }

        public bool IsConnected
        {
            get { return !m_Dispose && m_IsConnected; }
            internal set { m_IsConnected = !m_Dispose && value; }
        }
        #endregion

        public void SendDisconnect()
        {
            if (m_Dispose)
            {
                return;
            }

            byte[] buffer = new byte[8];
            ByteWriteMemory write = new ByteWriteMemory(buffer);
            write.Write(KcpConstants.Flag_Disconnect);
            write.Write(m_Conv);
            for (int i = 0; i < 3; ++i)
            {
                Send(buffer, 0, 8);
                Flush();
            }
        }
    }
}
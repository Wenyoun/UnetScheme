using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Net.KcpImpl;

namespace Nice.Game.Base
{
    internal abstract class KcpCon : IKcpCallback, IRentable, IDisposable
    {
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

        protected Socket m_Socket;
        protected EndPoint m_Point;

        private Kcp m_Kcp;
        private uint m_Conv;
        private uint m_ConId;

        private bool m_Dispose;
        private bool m_IsConnected;
        private byte[] m_OutputBuffer;

        protected KcpCon(uint conId, uint conv, Socket socket, EndPoint point)
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

        public void Output(Memory<byte> memory, int length)
        {
            if (m_Dispose)
            {
                return;
            }

            KcpHelper.Encode32u(m_OutputBuffer, 0, m_ConId);
            m_OutputBuffer[KcpConstants.Conv_Size] = MsgChannel.Reliable;
            memory.Span.Slice(0, length).CopyTo(new Span<byte>(m_OutputBuffer, KcpConstants.Head_Size, length));

            OnSendData(m_OutputBuffer, 0, length + KcpConstants.Head_Size);
        }


        public int Send(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -10;
            }
            return m_Kcp.Send(new Span<byte>(buffer, offset, length));
        }

        public int RawSend(byte[] buffer, int offset, int length)
        {
            if (m_Dispose)
            {
                return -10;
            }

            KcpHelper.Encode32u(m_OutputBuffer, 0, m_ConId);
            m_OutputBuffer[KcpConstants.Conv_Size] = MsgChannel.Unreliable;
            Array.Copy(buffer, offset, m_OutputBuffer, KcpConstants.Head_Size, length);

            OnSendData(m_OutputBuffer, 0, length + KcpConstants.Conv_Size + 1);

            return length + KcpConstants.Head_Size;
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

        internal void OnUpdate(long time)
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

        public uint ConId
        {
            get { return m_ConId; }
        }

        public bool IsConnected
        {
            get { return !m_Dispose && m_IsConnected; }
            internal set { m_IsConnected = !m_Dispose && value; }
        }
        #endregion

        internal void SendDisconnect()
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
            }
            Flush();
        }

        protected abstract void OnSendData(byte[] data, int offset, int size);
    }
}
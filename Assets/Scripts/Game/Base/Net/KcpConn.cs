using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Net.KcpImpl;
using UnityEngine;

namespace Zyq.Game.Base
{
    public class KcpConn : IKcpCallback, IDisposable
    {
        private Kcp kcp;
        private uint conv;
        private long conId;
        private Socket udp;
        private EndPoint point;
        private byte[] outputBuffer;
        private volatile bool isDispose;
        private volatile bool isConnected;

        public KcpConn(uint conv, Socket udp)
        {
            this.udp = udp;
            this.conv = conv;
            this.conId = conv;
            this.point = null;
            this.kcp = new Kcp(conv, this);
            this.isDispose = false;
            this.isConnected = false;
            this.outputBuffer = new byte[KcpHelper.Length];
            this.kcp.NoDelay(1, 10, 2, 1);
        }

        public KcpConn(long conId, uint conv, Socket udp, EndPoint point)
        {
            this.udp = udp;
            this.conv = conv;
            this.conId = conId;
            this.point = point;
            this.kcp = new Kcp(conv, this);
            this.isDispose = false;
            this.isConnected = false;
            this.outputBuffer = new byte[KcpHelper.Length];
            this.kcp.NoDelay(1, 10, 2, 1);
        }

        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }

        public void Output(IMemoryOwner<byte> memory, int length)
        {
            if (!isDispose)
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
            if (!isDispose)
            {
                return kcp.Send(new Span<byte>(buffer, offset, length));
            }

            return -10;
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            if (!isDispose)
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
                }
            }
        }

        #region internal

        internal void Flush()
        {
            if (!isDispose)
            {
                kcp.Flush();
            }
        }

        internal void Update(DateTime now)
        {
            if (!isDispose)
            {
                kcp.Update(now);
            }
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (!isDispose)
            {
                kcp.Input(new Span<byte>(buffer, offset, length));
            }
        }

        #endregion

        #region properties

        public uint Conv
        {
            get { return conv; }
        }

        public long ConId
        {
            get { return conId; }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            internal set { isConnected = value; }
        }

        #endregion
    }
}
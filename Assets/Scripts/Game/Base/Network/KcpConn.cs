﻿using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Net.KcpImpl;
using UnityEngine;

namespace Zyq.Game.Base
{
    public class KcpConn : IKcpCallback, IRentable, IDisposable
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

        private Kcp kcp;
        private Socket socket;
        private EndPoint point;

        private uint conv;
        private int conId;

        private bool isDispose;
        private bool isConnected;
        private byte[] outputBuffer;

        public KcpConn(uint conv, Socket socket) : this(conv, socket, null)
        {
        }

        public KcpConn(uint conv, Socket socket, EndPoint point)
        {
            this.socket = socket;
            this.point = point;

            this.conv = conv;
            this.conId = (int) conv;

            isDispose = false;
            isConnected = false;

            outputBuffer = new byte[KcpConstants.Packet_Length];
            kcp = new Kcp(conv, this, this);
            kcp.NoDelay(1, 10, 2, 1);
        }

        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return new MemoryPool(length);
        }

        public void Output(IMemoryOwner<byte> owner, int length)
        {
            if (isDispose)
            {
                return;
            }

            Memory<byte> memory = owner.Memory;
            KcpHelper.Encode32u(outputBuffer, 0, conv);
            memory.Span.Slice(0, length).CopyTo(new Span<byte>(outputBuffer, 4, outputBuffer.Length - 4));

            if (point == null)
            {
                socket.Send(outputBuffer, length + 4, SocketFlags.None);
            }
            else
            {
                socket.SendTo(outputBuffer, length + 4, SocketFlags.None, point);
            }
        }

        public int Send(byte[] buffer, int offset, int length)
        {
            if (isDispose)
            {
                return -10;
            }

            return kcp.Send(new Span<byte>(buffer, offset, length));
        }

        public int Recv(byte[] buffer, int offset, int length)
        {
            if (isDispose)
            {
                return -10;
            }

            return kcp.Recv(new Span<byte>(buffer, offset, length));

        }

        public void Dispose()
        {
            lock (this)
            {
                if (isDispose)
                {
                    return;
                }

                isDispose = true;
            }

            isConnected = false;
            kcp.Dispose();

            kcp = null;
            socket = null;
            point = null;
            outputBuffer = null;
        }

        #region internal
        internal void Flush()
        {
            if (isDispose)
            {
                return;
            }

            kcp.Flush();
        }

        internal void Update(long time)
        {
            if (isDispose)
            {
                return;
            }

            kcp.Update(time);
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (isDispose)
            {
                return;
            }

            kcp.Input(new Span<byte>(buffer, offset, length));
        }
        #endregion

        #region properties
        public uint Conv
        {
            get { return conv; }
        }

        public int ConId
        {
            get { return conId; }
        }

        public bool IsConnected
        {
            get { return !isDispose && isConnected; }
            internal set
            {
                if (isDispose)
                {
                    return;
                }

                isConnected = value;
            }
        }
        #endregion

        public void SendDisconnect()
        {
            if (isDispose)
            {
                return;
            }

            byte[] buffer = new byte[8];
            ByteWriteMemory write = new ByteWriteMemory(buffer);
            write.Write(KcpConstants.Flag_Disconnect);
            write.Write(conv);
            Send(buffer, 0, 8);
            Flush();
        }
    }
}
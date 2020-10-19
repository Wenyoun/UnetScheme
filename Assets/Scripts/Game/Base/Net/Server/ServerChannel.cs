using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace Zyq.Game.Base
{
    public class ServerChannel : AbstractChannel
    {
        private bool isClose;
        private bool isDispose;

        private KcpConn con;
        private ConcurrentQueue<Packet> recvPacketQueue;
        private ConcurrentQueue<Packet> sendPacketQueue;

        public ServerChannel(KcpConn con)
        {
            this.con = con;

            isClose = false;
            isDispose = false;

            recvPacketQueue = new ConcurrentQueue<Packet>();
            sendPacketQueue = new ConcurrentQueue<Packet>();
        }

        public override long ChannelId
        {
            get { return !isDispose ? con.ConId : -1; }
        }

        public override bool IsConnected
        {
            get { return !isDispose && con.IsConnected; }
        }

        public override void Disconnect()
        {
            if (isDispose)
            {
                return;
            }

            isClose = true;
        }

        public override void Dispatcher()
        {
            if (isDispose)
            {
                return;
            }

            if (recvPacketQueue.TryDequeue(out Packet packet))
            {
                ushort cmd = packet.Cmd;
                ByteBuffer byteBuffer = packet.Buffer;
                if (handlers.TryGetValue(packet.Cmd, out ChannelMessageDelegate handler))
                {
                    handler(new ChannelMessage(cmd, byteBuffer, this));
                }
            }
        }

        public override void Send(ushort cmd, ByteBuffer buffer)
        {
            if (isDispose)
            {
                return;
            }

            sendPacketQueue.Enqueue(new Packet(cmd, buffer));
        }

        public override void Dispose()
        {
            lock (this)
            {
                if (isDispose)
                {
                    return;
                }

                isDispose = true;
            }

            base.Dispose();

            con.Dispose();
            con = null;

            sendPacketQueue.Clear();
            sendPacketQueue = null;

            recvPacketQueue.Clear();
            recvPacketQueue = null;
        }

        #region internal method

        internal int Send(byte[] buffer, int offset, int length)
        {
            if (isDispose)
            {
                return -20;
            }

            return con.Send(buffer, offset, length);
        }

        internal int Recv(byte[] buffer, int offset, int length)
        {
            if (isDispose)
            {
                return -20;
            }

            return con.Recv(buffer, offset, length);
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            if (isDispose)
            {
                return;
            }

            con.Input(buffer, offset, length);
        }

        internal void Update(DateTime time)
        {
            if (isDispose)
            {
                return;
            }

            con.Update(time);
        }

        internal void ProcessSendPacket(ServerDataProcessingCenter process)
        {
            if (isDispose)
            {
                return;
            }

            process.TryParseSendKcpData(this, sendPacketQueue);
        }

        internal void ProcessRecvPacket(ServerDataProcessingCenter process, List<Packet> packets, IKcpConnect connect)
        {
            if (isDispose)
            {
                return;
            }

            if (process.TryParseRecvKcpData(this, packets, connect))
            {
                for (int i = 0; i < packets.Count; ++i)
                {
                    recvPacketQueue.Enqueue(packets[i]);
                }
                packets.Clear();
            }
        }

        internal void SetConnectedStatus(bool status)
        {
            if (isDispose)
            {
                return;
            }

            con.IsConnected = status;
        }

        internal uint Conv
        {
            get { return !isDispose ? con.Conv : 0; }
        }

        internal bool IsClose
        {
            get { return isDispose || isClose; }
        }

        #endregion
    }
}
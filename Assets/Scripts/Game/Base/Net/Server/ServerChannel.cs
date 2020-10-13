using System;
using System.Collections.Concurrent;

namespace Zyq.Game.Base
{
    public class ServerChannel : AbstractChannel
    {
        private KcpConn con;
        private bool isClose;
        private ConcurrentQueue<Packet> recvPacketQueue;
        private ConcurrentQueue<Packet> sendPacketQueue;

        public ServerChannel(KcpConn con)
        {
            this.con = con;
            this.isClose = false;
            this.recvPacketQueue = new ConcurrentQueue<Packet>();
            this.sendPacketQueue = new ConcurrentQueue<Packet>();
        }

        public override long ChannelId
        {
            get { return con.ConId; }
        }

        public override bool IsConnected
        {
            get { return con.IsConnected; }
        }

        public override void Disconnect()
        {
            isClose = true;
        }

        public override void Dispatcher()
        {
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

        public override void Send(Packet packet)
        {
            sendPacketQueue.Enqueue(packet);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (con != null)
            {
                con.Dispose();
            }

            ClearQueue(sendPacketQueue);

            ClearQueue(recvPacketQueue);
        }

        private void ClearQueue(ConcurrentQueue<Packet> queue)
        {
            while (queue.TryDequeue(out Packet packet))
            {
            }
        }

        #region internal method

        internal int Send(byte[] buffer, int offset, int length)
        {
            return con.Send(buffer, offset, length);
        }

        internal int Recv(byte[] buffer, int offset, int length)
        {
            return con.Recv(buffer, offset, length);
        }

        internal void Input(byte[] buffer, int offset, int length)
        {
            con.Input(buffer, offset, length);
        }

        internal void Update(DateTime time)
        {
            con.Update(time);
        }

        internal void ProcessSendPacket(ServerDataProcessingCenter process)
        {
            process.TrySendKcpData(this, sendPacketQueue);
        }

        internal void ProcessRecvPacket(ServerDataProcessingCenter process, IKcpConnect kcpConnect)
        {
            if (process.TryRecvKcpData(this, out Packet packet, kcpConnect))
            {
                recvPacketQueue.Enqueue(packet);
            }
        }

        internal uint Conv
        {
            get { return con.Conv; }
        }

        internal bool IsClose
        {
            get { return isClose; }
        }

        internal void SetConnectedStatus(bool status)
        {
            con.IsConnected = status;
        }

        #endregion
    }
}
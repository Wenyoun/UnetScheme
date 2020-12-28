using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zyq.Game.Base
{
    public class ServerChannel : AbsChannel
    {
        private bool isClose;
        private bool isDispose;
        private uint conv;
        private long conId;

        private KcpConn con;
        private ConcurrentQueue<Packet> recvPacketQueue;
        private ConcurrentQueue<Packet> sendPacketQueue;

        private ServerHeartbeatProcessing heartbeat;

        public ServerChannel(KcpConn con)
        {
            this.con = con;
            conv = con.Conv;
            this.conId = con.ConId;

            isClose = false;
            isDispose = false;

            recvPacketQueue = new ConcurrentQueue<Packet>();
            sendPacketQueue = new ConcurrentQueue<Packet>();

            heartbeat = new ServerHeartbeatProcessing();
        }

        public override long ChannelId
        {
            get { return conId; }
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
                    handler.Invoke(new ChannelMessage(cmd, byteBuffer));
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

            con.SendDisconnect();
            con.Dispose();

            sendPacketQueue.Clear();
            recvPacketQueue.Clear();
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

        internal void Update(long time)
        {
            if (isDispose)
            {
                return;
            }

            con.Update(time);
        }

        internal void Flush()
        {
            if (isDispose)
            {
                return;
            }

            con.Flush();
        }

        internal void ProcessSendPacket(ServerDataProcessingCenter process)
        {
            if (isDispose)
            {
                return;
            }

            process.TryParseSendKcpData(this, sendPacketQueue);
        }

        internal void ProcessRecvPacket(ServerDataProcessingCenter process, List<Packet> packets, IKcpConnect connectCallback)
        {
            if (isDispose)
            {
                return;
            }

            heartbeat.Tick(this);

            if (process.TryParseRecvKcpData(this, packets, connectCallback, heartbeat))
            {
                heartbeat.UpdateHeartbeat();
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
            get { return conv; }
        }

        internal bool IsClose
        {
            get { return isDispose || isClose; }
        }

        #endregion
    }
}
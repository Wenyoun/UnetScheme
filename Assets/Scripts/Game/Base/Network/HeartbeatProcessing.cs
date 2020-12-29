﻿namespace Zyq.Game.Base
{
    internal class HeartbaetConstants
    {
        public const int Send_Interval_Mills = 5000;
        public const int Timeout_Interval_Mills = 15000;
    }

    internal class ClientHeartbeatProcessing
    {
        private long recvMills;
        private long sendMills;
        private byte[] rawBuffer;

        public ClientHeartbeatProcessing()
        {
            long current = TimeUtil.Get1970ToNowMilliseconds();
            recvMills = current;
            sendMills = current - 1000000;
            rawBuffer = new byte[32];
        }

        public void UpdateHeartbeat()
        {
            recvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void OnUpdate(KcpUdpClient client, KcpConn con, long time)
        {
            long current = time;

            if (time - recvMills >= HeartbaetConstants.Timeout_Interval_Mills)
            {
                client.Dispose();
                return;
            }

            if (current - sendMills >= HeartbaetConstants.Send_Interval_Mills)
            {
                sendMills = current;
                ByteWriteMemory write = new ByteWriteMemory(rawBuffer);
                write.Write(KcpConstants.Flag_Heartbeat);
                write.Write(con.Conv);
                con.Send(rawBuffer, 0, 8);
            }
        }
    }

    internal class ServerHeartbeatProcessing
    {
        private long recvMills;

        public ServerHeartbeatProcessing()
        {
            recvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void UpdateHeartbeat()
        {
            recvMills = TimeUtil.Get1970ToNowMilliseconds();
        }

        public void UpdateHeartbeat(ServerChannel channel, byte[] rawBuffer, int ofsset, int length)
        {
            recvMills = TimeUtil.Get1970ToNowMilliseconds();
            channel.Send(rawBuffer, ofsset, length);
        }

        public void OnUpdate(ServerChannel channel)
        {
            long current = TimeUtil.Get1970ToNowMilliseconds();
            if (current - recvMills > HeartbaetConstants.Timeout_Interval_Mills)
            {
                channel.Disconnect();
            }
        }
    }
}
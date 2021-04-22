using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace Nice.Game.Base
{
    internal class KcpConstants
    {
        public const int Head_Size = 8;
        public const int Packet_Length = 1300;
        public const uint Flag_Connect = 0xfffffff0;
        public const uint Flag_Heartbeat = 0xfffffff1;
        public const uint Flag_Disconnect = 0xffffffff;
    }

    internal static class KcpHelper
    {
        internal static int Encode32u(byte[] p, int offset, uint w)
        {
            p[0 + offset] = (byte) (w >> 0);
            p[1 + offset] = (byte) (w >> 8);
            p[2 + offset] = (byte) (w >> 16);
            p[3 + offset] = (byte) (w >> 24);
            return 4;
        }

        internal static uint Decode32u(byte[] p, int offset)
        {
            uint result = 0;
            result |= (uint) (p[0 + offset] << 0);
            result |= (uint) (p[1 + offset] << 8);
            result |= (uint) (p[2 + offset] << 16);
            result |= (uint) (p[3 + offset] << 24);
            return result;
        }

        internal static void Encode64(byte[] p, int offset, long w)
        {
            p[0 + offset] = (byte) (w >> 0);
            p[1 + offset] = (byte) (w >> 8);
            p[2 + offset] = (byte) (w >> 16);
            p[3 + offset] = (byte) (w >> 24);
            p[4 + offset] = (byte) (w >> 32);
            p[5 + offset] = (byte) (w >> 40);
            p[6 + offset] = (byte) (w >> 48);
            p[7 + offset] = (byte) (w >> 56);
        }

        internal static long Decode64(byte[] p, int offset)
        {
            long result = 0;
            result |= p[0 + offset] << 0;
            result |= p[1 + offset] << 8;
            result |= p[2 + offset] << 16;
            result |= p[3 + offset] << 24;
            result |= (long) p[4 + offset] << 32;
            result |= (long) p[5 + offset] << 40;
            result |= (long) p[6 + offset] << 48;
            result |= (long) p[7 + offset] << 56;
            return result;
        }

        internal static void Clear(this ConcurrentQueue<Packet> queue)
        {
            while (queue.TryDequeue(out Packet packet))
            {
            }
        }

        internal static void CreateThread(ParameterizedThreadStart start, object obj = null)
        {
            Thread thread = new Thread(start);
            thread.Priority = ThreadPriority.Highest;
            thread.IsBackground = true;
            thread.Start(obj);
        }
    }
}
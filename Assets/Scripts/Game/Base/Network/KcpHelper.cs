using System.Collections.Concurrent;
using System.Threading;

namespace Zyq.Game.Base
{
    internal class KcpConstants
    {
        public const int Length = 1500;
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
            result |= (uint) p[0 + offset];
            result |= (uint) (p[1 + offset] << 8);
            result |= (uint) (p[2 + offset] << 16);
            result |= (uint) (p[3 + offset] << 24);
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
            thread.Start(obj);
        }
    }
}
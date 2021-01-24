using Nice.Game.Base;

namespace Nice.Game.Client
{
    public static class NetworkClient
    {
        private static ClientChannel channel;

        public static void Init(IClientCallback callback)
        {
            channel = new ClientChannel(callback);
        }

        public static void Connect(string host, int port)
        {
            if (channel != null)
            {
                channel.Connect(host, port);
            }
        }

        public static void Disconnect()
        {
            if (channel != null)
            {
                channel.Disconnect();
            }
        }

        public static void Dispatcher()
        {
            if (channel != null)
            {
                channel.Dispatcher();
            }
        }

        public static void Send(ushort cmd, ByteBuffer buffer)
        {
            if (channel != null)
            {
                channel.Send(cmd, buffer);
            }
        }

        public static void Dispose()
        {
            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }
        }
    }
}
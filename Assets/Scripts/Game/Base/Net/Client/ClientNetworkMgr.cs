namespace Zyq.Game.Base
{
    public class ClientNetworkMgr
    {
        private static ClientChannel channel;

        public static void Dispose()
        {
            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }
        }

        public static void Dispatcher()
        {
            if (channel != null)
            {
                channel.Dispatcher();
            }
        }

        public static void Connect(string host, int port, IClientCallback client)
        {
            if (channel == null)
            {
                channel = new ClientChannel(client);
                channel.Connect(host, port);
            }
        }
    }
}
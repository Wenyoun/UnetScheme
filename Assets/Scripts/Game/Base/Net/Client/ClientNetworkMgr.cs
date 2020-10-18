using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class ClientNetworkMgr
    {
        private static List<IChannel> channels = new List<IChannel>();

        public static void Dispose()
        {
            int length = channels.Count;
            for (int i = 0; i < length; ++i)
            {
                channels[i].Dispose();
            }
            channels.Clear();
        }

        public static void Dispatcher()
        {
            int length = channels.Count;
            for (int i = 0; i < length; ++i)
            {
                channels[i].Dispatcher();
            }
        }

        public static void Connect(string host, int port, IClientCallback client)
        {
            ClientChannel channel = new ClientChannel(client);
            channel.Connect(host, port);
            channels.Add(channel);
        }

        public static void Disconnect()
        {
            Dispose();
        }
    }
}
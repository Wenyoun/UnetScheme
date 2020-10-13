using System;

namespace Zyq.Game.Base
{
    public class ClientNetworkMgr
    {
        private static ClientNetwork clientNetwork;

        private ClientNetworkMgr()
        {
        }

        public static void Init(IClient client)
        {
            if (clientNetwork != null)
            {
                throw new ArgumentException("ClientNetworkMgr already init");
            }

            clientNetwork = new ClientNetwork(client);
        }
        
        public static void Dispose()
        {
            if (clientNetwork != null)
            {
                clientNetwork.Disconnect();
                clientNetwork = null;
            }
        }

        public static void Dispatcher()
        {
            if (clientNetwork != null)
            {
                clientNetwork.Dispatcher();
            }
        }

        public static void Connect(string host, int port)
        {
            if (clientNetwork != null)
            {
                clientNetwork.Connect(host, port);
            }
        }

        public static void Disconnect()
        {
            if (clientNetwork != null)
            {
                clientNetwork.Disconnect();
            }
        }
    }
}
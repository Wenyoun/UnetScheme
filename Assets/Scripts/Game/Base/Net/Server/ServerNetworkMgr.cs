using System;

namespace Zyq.Game.Base
{
    public class ServerNetworkMgr
    {
        private static ServerNetwork network;
        
            
        public static void Init(IServer server)
        {
            if (network != null)
            {
                throw new ArgumentException("ClientNetworkMgr already init");
            }

            network = new ServerNetwork(server);
        }
        
        public static void Bind(int port)
        {
            if (network != null)
            {
                network.Bind(port);
            }
        }

        public static void Close(long channelId)
        {
            if (network != null)
            {
                network.Close(channelId);
            }
        }

        public static void OnUpdate()
        {
            if (network != null)
            {
                network.OnUpdate();
            }
        }
        
        public static void Dispose()
        {
            if (network != null)
            {
                network.Dispose();
                network = null;
            }
        }
    }
}
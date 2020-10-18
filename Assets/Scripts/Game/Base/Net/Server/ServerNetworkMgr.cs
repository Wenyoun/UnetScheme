namespace Zyq.Game.Base
{
    public class ServerNetworkMgr
    {
        private static ServerNetwork network;
        
        public static void Bind(int port, IServerCallback callback)
        {
            if (network == null)
            {
                network = new ServerNetwork(callback);
                network.Bind(port);
            }
        }

        public static void CloseChannel(uint channelId)
        {
            if (network != null)
            {
                network.CloseChannel(channelId);
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
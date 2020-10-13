using System;

namespace Zyq.Game.Base
{
    internal class ClientNetwork : IDisposable
    {
        private ClientChannel channel;

        public ClientNetwork(IClient client)
        {
            channel = new ClientChannel(client);
        }

        public void Connect(string host, int port)
        {
            channel?.Connect(host, port);
        }

        public void Disconnect()
        {
            channel?.Disconnect();
        }

        public void Dispatcher()
        {
            channel?.Dispatcher();
        }

        public void Dispose()
        {
            channel?.Dispose();
            channel = null;
        }
    }
}
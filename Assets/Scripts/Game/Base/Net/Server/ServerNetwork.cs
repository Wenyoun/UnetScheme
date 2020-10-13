using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class ServerNetwork : IDisposable
    {
        private IServer serverCallback;
        private KcpUdpServer kcpUdpServer;
        private Dictionary<long, IChannel> channels;

        public ServerNetwork(IServer serverCallback)
        {
            this.serverCallback = serverCallback;
            kcpUdpServer = new KcpUdpServer();
            channels = new Dictionary<long, IChannel>();
        }

        public void Bind(int port)
        {
            kcpUdpServer.Bind(port, new KcpConnect(OnKcpConnect, OnKcpDisconnect));
        }

        public void Close(long channelId)
        {
            if (channels.TryGetValue(channelId, out IChannel channel))
            {
                channels.Remove(channelId);
                channel.Disconnect();
            }
        }

        public void OnUpdate()
        {
            Dictionary<long, IChannel>.Enumerator its = channels.GetEnumerator();
            while (its.MoveNext())
            {
                its.Current.Value.Dispatcher();
            }
        }

        public void Dispose()
        {
            channels.Clear();
            kcpUdpServer.Dispose();
        }

        private void OnKcpConnect(IChannel channel)
        {
            if (!channels.ContainsKey(channel.ChannelId))
            {
                channels.Add(channel.ChannelId, channel);
                if (serverCallback != null)
                {
                    serverCallback.OnClientConnect(channel);
                }
            }
        }

        private void OnKcpDisconnect(IChannel channel)
        {
            if (channels.ContainsKey(channel.ChannelId))
            {
                if (serverCallback != null)
                {
                    serverCallback.OnClientDisconnect(channel);
                }

                channels.Remove(channel.ChannelId);
            }
        }

        private class KcpConnect : IKcpConnect
        {
            private Action<IChannel> connect;
            private Action<IChannel> disconnect;

            public KcpConnect(Action<IChannel> connect, Action<IChannel> disconnect)
            {
                this.connect = connect;
                this.disconnect = disconnect;
            }

            public void OnKcpConnect(IChannel channel)
            {
                if (connect != null)
                {
                    connect(channel);
                }
            }

            public void OnKcpDisconnect(IChannel channel)
            {
                if (disconnect != null)
                {
                    disconnect(channel);
                }
            }
        }
    }
}
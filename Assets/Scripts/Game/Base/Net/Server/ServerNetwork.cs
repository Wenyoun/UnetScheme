using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class ServerNetwork : IDisposable
    {
        private bool isDispose;
        private IServerCallback serverCallback;

        private KcpUdpServer kcpUdpServer;

        private Dictionary<long, IChannel> channels;
        private ConcurrentQueue<StatusChannel> statusChannels;

        public ServerNetwork(IServerCallback callback)
        {
            isDispose = false;
            serverCallback = callback;

            kcpUdpServer = new KcpUdpServer();

            channels = new Dictionary<long, IChannel>();
            statusChannels = new ConcurrentQueue<StatusChannel>();
        }

        public void Bind(int port)
        {
            if (isDispose)
            {
                return;
            }

            kcpUdpServer.Bind(port, new KcpConnect(OnKcpConnect, OnKcpDisconnect));
        }

        public void CloseChannel(uint channelId)
        {
            if (isDispose)
            {
                return;
            }

            if (channels.TryGetValue(channelId, out IChannel channel))
            {
                channels.Remove(channelId);
                channel.Disconnect();
            }
        }

        public void OnUpdate()
        {
            if (isDispose)
            {
                return;
            }

            CheckStatusChannels();

            Dispatcher();
        }

        public void Dispose()
        {
            if (isDispose)
            {
                return;
            }

            isDispose = true;
            channels.Clear();
            kcpUdpServer.Dispose();
        }

        private void OnKcpConnect(IChannel channel)
        {
            if (isDispose)
            {
                return;
            }

            statusChannels.Enqueue(new StatusChannel(Status.Add, channel));
        }

        private void OnKcpDisconnect(IChannel channel)
        {
            if (isDispose)
            {
                return;
            }

            statusChannels.Enqueue(new StatusChannel(Status.Remove, channel));
        }

        private void Dispatcher()
        {
            Dictionary<long, IChannel>.Enumerator its = channels.GetEnumerator();
            while (its.MoveNext())
            {
                its.Current.Value.Dispatcher();
            }
        }

        private void CheckStatusChannels()
        {
            while (statusChannels.TryDequeue(out StatusChannel status))
            {
                IChannel channel = status.Channel;

                if (status.Status == Status.Add)
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
                else if (status.Status == Status.Remove)
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
            }
        }

        private enum Status
        {
            Add,
            Remove
        }

        private struct StatusChannel
        {
            public Status Status;
            public IChannel Channel;

            public StatusChannel(Status status, IChannel channel)
            {
                Status = status;
                Channel = channel;
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
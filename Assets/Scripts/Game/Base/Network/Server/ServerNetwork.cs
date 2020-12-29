using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class ServerNetwork : IDisposable
    {
        private bool m_Dispose;
        private IServerCallback m_Callback;
        private KcpUdpServer m_KcpUdpServer;
        private Dictionary<long, IChannel> m_Channels;
        private ConcurrentQueue<StatusChannel> m_StatusChannels;

        public ServerNetwork(IServerCallback callback)
        {
            m_Dispose = false;
            m_Callback = callback;
            m_KcpUdpServer = new KcpUdpServer();
            m_Channels = new Dictionary<long, IChannel>();
            m_StatusChannels = new ConcurrentQueue<StatusChannel>();
        }

        public void Dispose()
        {
            if (m_Dispose)
            {
                return;
            }

            m_Dispose = true;
            m_Channels.Clear();
            m_KcpUdpServer.Dispose();

            while (m_StatusChannels.TryDequeue(out StatusChannel channel))
            {
            }
        }

        public void Bind(int port)
        {
            if (m_Dispose)
            {
                return;
            }

            m_KcpUdpServer.Bind(port, new KcpConnect(OnKcpConnect, OnKcpDisconnect));
        }

        public void CloseChannel(int channelId)
        {
            if (m_Dispose)
            {
                return;
            }

            if (m_Channels.TryGetValue(channelId, out IChannel channel))
            {
                m_Channels.Remove(channelId);
                channel.Disconnect();
            }
        }

        public void OnUpdate()
        {
            if (m_Dispose)
            {
                return;
            }

            CheckStatusChannels();
            HandlePackets();
        }

        private void OnKcpConnect(IChannel channel)
        {
            if (m_Dispose)
            {
                return;
            }

            m_StatusChannels.Enqueue(new StatusChannel(Status.Add, channel));
        }

        private void OnKcpDisconnect(IChannel channel)
        {
            if (m_Dispose)
            {
                return;
            }

            m_StatusChannels.Enqueue(new StatusChannel(Status.Remove, channel));
        }

        private void HandlePackets()
        {
            using (Dictionary<long, IChannel>.Enumerator its = m_Channels.GetEnumerator())
            {
                while (its.MoveNext())
                {
                    its.Current.Value.Dispatcher();
                }
            }
        }

        private void CheckStatusChannels()
        {
            while (m_StatusChannels.TryDequeue(out StatusChannel status))
            {
                IChannel channel = status.Channel;

                if (status.Status == Status.Add)
                {
                    if (!m_Channels.ContainsKey(channel.ChannelId))
                    {
                        m_Channels.Add(channel.ChannelId, channel);

                        if (m_Callback != null)
                        {
                            m_Callback.OnClientConnect(channel);
                        }
                    }
                }
                else if (status.Status == Status.Remove)
                {
                    if (m_Channels.ContainsKey(channel.ChannelId))
                    {
                        if (m_Callback != null)
                        {
                            m_Callback.OnClientDisconnect(channel);
                        }

                        m_Channels.Remove(channel.ChannelId);
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
            private Action<IChannel> m_Connect;
            private Action<IChannel> m_Disconnect;

            public KcpConnect(Action<IChannel> connect, Action<IChannel> disconnect)
            {
                m_Connect = connect;
                m_Disconnect = disconnect;
            }

            public void OnKcpConnect(IChannel channel)
            {
                m_Connect?.Invoke(channel);
            }

            public void OnKcpDisconnect(IChannel channel)
            {
                m_Disconnect?.Invoke(channel);
            }
        }
    }
}
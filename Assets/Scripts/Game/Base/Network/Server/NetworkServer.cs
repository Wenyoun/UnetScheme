using System;

namespace Nice.Game.Base
{
    internal class NetworkServer : IDisposable, IChannelListener
    {
        private bool m_Dispose;
        private IConnectionHandle m_Handle;
        private ServerTransport m_Transport;
        private HDictionary<uint, Connection> m_Connections;
        private HConcurrentQueue<WrapChannel> m_WrapChannels;

        public NetworkServer()
        {
            m_Dispose = false;
            m_Transport = new ServerTransport();
            m_Connections = new HDictionary<uint, Connection>();
            m_WrapChannels = new HConcurrentQueue<WrapChannel>();
        }

        public void Dispose()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Dispose = true;

            foreach (Connection connection in m_Connections)
            {
                connection.Dispose();
            }

            m_Connections.Clear();
            m_WrapChannels.Clear();
            m_Transport.Dispose();
            m_Handle = null;
        }

        public void Bind(int port, IConnectionHandle handle)
        {
            if (m_Dispose)
            {
                throw new ObjectDisposedException("NetworkServer already disposed!");
            }

            m_Handle = handle;
            m_Transport.Bind(port, this);
        }

        public void Send(uint connectionId, ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (m_Dispose)
            {
                return;
            }

            if (m_Connections.TryGetValue(connectionId, out Connection connection))
            {
                connection.Send(cmd, buffer, channel);
            }
        }

        public void OnUpdate()
        {
            if (m_Dispose)
            {
                return;
            }

            CheckWrapChannels();

            foreach (Connection connection in m_Connections)
            {
                connection.OnUpdate();
            }
        }

        public void OnAddChannel(IChannel channel)
        {
            if (m_Dispose)
            {
                return;
            }

            m_WrapChannels.Enqueue(new WrapChannel(WrapChannel.ADD, channel));
        }

        public void OnRemoveChannel(IChannel channel)
        {
            if (m_Dispose)
            {
                return;
            }

            m_WrapChannels.Enqueue(new WrapChannel(WrapChannel.REMOVE, channel));
        }

        private void CheckWrapChannels()
        {
            if (!m_WrapChannels.IsEmpty)
            {
                while (m_WrapChannels.TryDequeue(out WrapChannel wrap))
                {
                    IChannel channel = wrap.Channel;
                    if (wrap.Flag == WrapChannel.ADD)
                    {
                        if (!m_Connections.ContainsKey(channel.ChannelId))
                        {
                            Connection connection = new Connection(channel);
                            m_Connections.Add(channel.ChannelId, connection);
                            if (m_Handle != null)
                            {
                                m_Handle.OnAddConnection(connection);
                            }
                        }
                    }
                    else if (wrap.Flag == WrapChannel.REMOVE)
                    {
                        if (m_Connections.TryGetValue(channel.ChannelId, out Connection connection))
                        {
                            m_Connections.Remove(channel.ChannelId);
                            if (m_Handle != null)
                            {
                                m_Handle.OnRemoveConnection(connection);
                            }
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        private struct WrapChannel
        {
            public const int ADD = 1;
            public const int REMOVE = 2;

            public int Flag;
            public IChannel Channel;

            public WrapChannel(int flag, IChannel channel)
            {
                Flag = flag;
                Channel = channel;
            }
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine.Experimental.PlayerLoop;

namespace Nice.Game.Base {
    public class NetworkServer : IDisposable {
        private bool m_Dispose;
        private IServerConnect m_Connect;
        private ServerTransport m_Transport;
        private Dictionary<uint, IChannel> m_Channels;
        private ConcurrentQueue<WrapperChannel> m_WrapperChannels;

        public NetworkServer() {
            m_Dispose = false;
            m_Transport = new ServerTransport();
            m_Channels = new Dictionary<uint, IChannel>();
            m_WrapperChannels = new ConcurrentQueue<WrapperChannel>();
        }

        public void Dispose() {
            if (m_Dispose) {
                return;
            }

            m_Dispose = true;
            m_Channels.Clear();
            m_Transport.Dispose();

            while (m_WrapperChannels.TryDequeue(out WrapperChannel channel)) {
            }
        }

        public void SetConnect(IServerConnect connect) {
            m_Connect = connect;
        }

        public void Bind(int port) {
            if (m_Dispose) {
                return;
            }

            m_Transport.Bind(port, new KcpConnect(OnKcpConnect, OnKcpDisconnect));
        }

        public void CloseChannel(uint channelId) {
            if (m_Dispose) {
                return;
            }

            if (m_Channels.TryGetValue(channelId, out IChannel channel)) {
                m_Channels.Remove(channelId);
                channel.Disconnect();
            }
        }

        public void OnUpdate() {
            if (m_Dispose) {
                return;
            }

            CheckWrapperChannels();
            OnHandlePackets();
        }

        private void OnKcpConnect(IChannel channel) {
            if (m_Dispose) {
                return;
            }

            m_WrapperChannels.Enqueue(new WrapperChannel(Status.Add, channel));
        }

        private void OnKcpDisconnect(IChannel channel) {
            if (m_Dispose) {
                return;
            }

            m_WrapperChannels.Enqueue(new WrapperChannel(Status.Remove, channel));
        }

        private void OnHandlePackets() {
            using (Dictionary<uint, IChannel>.Enumerator its = m_Channels.GetEnumerator()) {
                while (its.MoveNext()) {
                    its.Current.Value.OnUpdate();
                }
            }
        }

        private void CheckWrapperChannels() {
            while (m_WrapperChannels.TryDequeue(out WrapperChannel wrapper)) {
                IChannel channel = wrapper.Channel;

                if (wrapper.Status == Status.Add) {
                    if (!m_Channels.ContainsKey(channel.ChannelId)) {
                        m_Channels.Add(channel.ChannelId, channel);
                        m_Connect?.OnConnect(channel);
                    }
                } else if (wrapper.Status == Status.Remove) {
                    if (m_Channels.ContainsKey(channel.ChannelId)) {
                        m_Channels.Remove(channel.ChannelId);
                        m_Connect?.OnDisconnect(channel);
                    }
                }
            }
        }

        private enum Status {
            Add,
            Remove
        }

        private struct WrapperChannel {
            public Status Status;
            public IChannel Channel;

            public WrapperChannel(Status status, IChannel channel) {
                Status = status;
                Channel = channel;
            }
        }

        private class KcpConnect : IKcpConnect {
            private Action<IChannel> m_Connect;
            private Action<IChannel> m_Disconnect;

            public KcpConnect(Action<IChannel> connect, Action<IChannel> disconnect) {
                m_Connect = connect;
                m_Disconnect = disconnect;
            }

            public void OnKcpConnect(IChannel channel) {
                m_Connect?.Invoke(channel);
            }

            public void OnKcpDisconnect(IChannel channel) {
                m_Disconnect?.Invoke(channel);
            }
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base {
    public interface IKcpConnect {
        void OnKcpConnect(IChannel channel);

        void OnKcpDisconnect(IChannel channel);
    }

    public class ServerSocket : IDisposable {
        private bool m_Dispose;
        private Socket m_Socket;
        private IKcpConnect m_Connect;
        private ServerDataProcessing m_Process;
        private ConcurrentDictionary<uint, ServerChannel> m_Channels;

        public ServerSocket() {
            m_Dispose = false;
            m_Socket = null;
            m_Connect = null;
            m_Process = new ServerDataProcessing();
            m_Channels = new ConcurrentDictionary<uint, ServerChannel>();
        }

        public void Bind(int port, IKcpConnect connect) {
            m_Connect = connect;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));

            KcpHelper.CreateThread(UpdateKcpLooper);
            KcpHelper.CreateThread(RecvUdpDataLooper);
        }

        public void Dispose() {
            if (m_Dispose) {
                return;
            }
            m_Dispose = true;

            IEnumerator<KeyValuePair<uint, ServerChannel>> its = m_Channels.GetEnumerator();
            while (its.MoveNext()) {
                its.Current.Value.Dispose();
            }
            m_Channels.Clear();

            if (m_Socket != null) {
                m_Socket.Dispose();
            }

            m_Socket = null;
            m_Connect = null;
            m_Process = null;
            m_Channels = null;
        }

        private void RecvUdpDataLooper(object obj) {
            try {
                uint startConvId = 10000;
                const int pollTimeout = 10000;
                byte[] rawBuffer = new byte[KcpConstants.Packet_Length];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                List<Packet> packets = new List<Packet>();

                while (!m_Dispose) {
                    if (!m_Socket.Poll(pollTimeout, SelectMode.SelectRead)) {
                        continue;
                    }

                    if (m_Dispose) {
                        break;
                    }

                    int count = m_Socket.ReceiveFrom(rawBuffer, SocketFlags.None, ref remote);

                    uint cid = (uint) remote.GetHashCode();
                    if (!m_Channels.TryGetValue(cid, out ServerChannel channel)) {
                        if (count == 4) {
                            uint flag = KcpHelper.Decode32u(rawBuffer, 0);
                            if (flag == KcpConstants.Flag_Connect) {
                                uint conv = startConvId++;
                                EndPoint point = remote.Create(remote.Serialize());
                                channel = new ServerChannel(new KcpConServer(cid, conv, m_Socket, point));

                                if (m_Channels.TryAdd(cid, channel)) {
                                    KcpHelper.Encode32u(rawBuffer, 0, KcpConstants.Flag_Connect);
                                    KcpHelper.Encode32u(rawBuffer, 4, conv);
                                    channel.Send(rawBuffer, 0, 8);
                                    channel.Flush();
                                }
                            }
                        } else {
                            Logger.Error($"not connect size={count}");
                        }
                    } else if (count > KcpConstants.Head_Size) {
                        uint remoteCid = KcpHelper.Decode32u(rawBuffer, 0);
                        if (cid == remoteCid) {
                            byte msgChannel = rawBuffer[KcpConstants.Conv_Size];
                            if (msgChannel == MsgChannel.Reliable) {
                                channel.Input(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size);
                                channel.RecvReliablePackets(m_Process, packets, m_Connect);
                            } else if (msgChannel == MsgChannel.Unreliable) {
                                channel.RecvUnreliablePackets(rawBuffer, KcpConstants.Head_Size, count - KcpConstants.Head_Size, m_Process, packets);
                            }
                        } else {
                            Logger.Error($"conv {cid} != {remoteCid}");
                        }
                    } else {
                        Logger.Error($"size={count} < {KcpConstants.Head_Size}");
                    }
                }
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        }

        private void UpdateKcpLooper(object obj) {
            try {
                List<uint> removes = new List<uint>();

                while (!m_Dispose) {
                    using (IEnumerator<KeyValuePair<uint, ServerChannel>> its = m_Channels.GetEnumerator()) {
                        while (its.MoveNext()) {
                            ServerChannel channel = its.Current.Value;
                            if (channel.IsDispose) {
                                removes.Add(channel.ChannelId);
                            } else {
                                channel.OnUpdate(m_Process);
                            }
                        }
                    }

                    int length = removes.Count;
                    if (length > 0) {
                        for (int i = 0; i < length; ++i) {
                            uint channelId = removes[i];
                            if (m_Channels.TryRemove(channelId, out ServerChannel channel)) {
                                m_Connect?.OnKcpDisconnect(channel);
                                channel.Dispose();
                            }
                        }
                        removes.Clear();
                    }

                    Thread.Sleep(1);
                }
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        }
    }
}
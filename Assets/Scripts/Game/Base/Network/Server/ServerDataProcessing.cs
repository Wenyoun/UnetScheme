﻿using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base {
    internal class ServerDataProcessing {
        private byte[] m_SendBuffer;
        private byte[] m_RecvBuffer;

        public ServerDataProcessing() {
            m_SendBuffer = new byte[ushort.MaxValue / 2];
            m_RecvBuffer = new byte[ushort.MaxValue / 2];
        }

        public bool RecvReliablePackets(ServerChannel channel, List<Packet> packets, IKcpConnect connect, ServerHeartbeatProcessing heartbeat) {
            while (true) {
                int size = channel.Recv(m_RecvBuffer, 0, m_RecvBuffer.Length);

                if (size <= 0) {
                    break;
                }

                if (size == 8) {
                    ByteReadMemory memory = new ByteReadMemory(m_RecvBuffer, 0, 8);
                    uint flag = memory.ReadUInt();
                    uint conv = memory.ReadUInt();

                    if (conv == channel.Conv) {
                        if (flag == KcpConstants.Flag_Connect) {
                            channel.SetConnectedStatus(true);
                            connect?.OnKcpConnect(channel);
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Disconnect) {
                            channel.SetConnectedStatus(false);
                            connect?.OnKcpDisconnect(channel);
                            channel.Disconnect();
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Heartbeat) {
                            heartbeat.UpdateHeartbeat(channel, m_RecvBuffer, 0, 8);
                            continue;
                        }
                    }
                }

                PacketProcessing.Recv(m_RecvBuffer, 0, size, packets);
            }

            return packets.Count > 0;
        }

        public bool RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, List<Packet> packets) {
            PacketProcessing.Recv(rawBuffer, offset, count, packets);
            return packets.Count > 0;
        }

        public void SendPackets(ServerChannel channel, ConcurrentQueue<Packet> packets) {
            while (packets.TryDequeue(out Packet packet)) {
                int size = PacketProcessing.Send(m_SendBuffer, packet);
                if (size > 0) {
                    if (packet.Channel == MsgChannel.Reliable) {
                        channel.Send(m_SendBuffer, 0, size);
                    } else if (packet.Channel == MsgChannel.Unreliable) {
                        channel.RawSend(m_SendBuffer, 0, size);
                    }
                }
            }
        }
    }
}
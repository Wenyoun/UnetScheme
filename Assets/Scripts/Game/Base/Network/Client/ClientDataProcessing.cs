using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base {
    internal class ClientDataProcessing {
        private byte[] m_Buffer;

        public ClientDataProcessing() {
            m_Buffer = new byte[ushort.MaxValue / 2];
        }

        public void SendPackets(KcpCon kcp, ConcurrentQueue<Packet> packets) {
            while (packets.TryDequeue(out Packet packet)) {
                int size = PacketProcessing.Send(m_Buffer, packet);
                if (size > 0) {
                    if (packet.Channel == ChannelType.Reliable) {
                        kcp.Send(m_Buffer, 0, size);
                    } else if (packet.Channel == ChannelType.Unreliable) {
                        kcp.RawSend(m_Buffer, 0, size);
                    }
                }
            }
        }

        public void RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, List<Packet> packets) {
            PacketProcessing.Recv(rawBuffer, offset, count, packets);
        }

        public void RecvReliablePackets(ClientTransport transport, KcpCon kcp, List<Packet> packets, ClientHeartbeatProcessing heartbeat) {
            while (true) {
                int size = kcp.Recv(m_Buffer, 0, m_Buffer.Length);

                if (size <= 0) {
                    break;
                }

                if (size == 8) {
                    ByteReadMemory memory = new ByteReadMemory(m_Buffer, 0, 8);
                    uint flag = memory.ReadUInt();
                    uint conv = memory.ReadUInt();

                    if (conv == kcp.Conv) {
                        if (flag == KcpConstants.Flag_Heartbeat) {
                            heartbeat.RecvHeartbeat();
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Disconnect) {
                            transport.Disconnect(false, true);
                            continue;
                        }
                    }
                }

                PacketProcessing.Recv(m_Buffer, 0, size, packets);
            }
        }
    }
}
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base {
    internal class ClientDataProcessing {
        private byte[] m_SendBuffer;
        private byte[] m_RecvBuffer;

        public ClientDataProcessing() {
            m_SendBuffer = new byte[ushort.MaxValue / 2];
            m_RecvBuffer = new byte[ushort.MaxValue / 2];
        }

        public void RecvReliablePackets(ClientTransport transport, KcpCon kcp, List<Packet> packets, ClientHeartbeatProcessing heartbeat) {
            while (true) {
                int size = kcp.Recv(m_RecvBuffer, 0, m_RecvBuffer.Length);

                if (size <= 0) {
                    break;
                }

                if (size == 8) {
                    ByteReadMemory memory = new ByteReadMemory(m_RecvBuffer, 0, 8);
                    uint flag = memory.ReadUInt();
                    uint conv = memory.ReadUInt();

                    if (conv == kcp.Conv) {
                        if (flag == KcpConstants.Flag_Heartbeat) {
                            heartbeat.UpdateHeartbeat();
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Disconnect) {
                            transport.Disconnect(false, true);
                            continue;
                        }
                    }
                }

                PacketProcessing.Recv(m_RecvBuffer, 0, size, packets);
            }
        }

        public void RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, List<Packet> packets) {
            PacketProcessing.Recv(rawBuffer, offset, count, packets);
        }

        public void SendPackets(KcpCon kcp, ConcurrentQueue<Packet> packets) {
            while (packets.TryDequeue(out Packet packet)) {
                int size = PacketProcessing.Send(m_SendBuffer, packet);
                if (size > 0) {
                    if (packet.Channel == MsgChannel.Reliable) {
                        kcp.Send(m_SendBuffer, 0, size);
                    } else if (packet.Channel == MsgChannel.Unreliable) {
                        kcp.RawSend(m_SendBuffer, 0, size);
                    }
                }
            }
        }
    }
}
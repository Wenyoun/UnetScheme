using System.Collections.Generic;

namespace Nice.Game.Base
{
    internal class ServerDataProcessing
    {
        private byte[] m_SendBuffer;
        private byte[] m_RecvBuffer;

        public ServerDataProcessing()
        {
            m_SendBuffer = new byte[ushort.MaxValue / 2];
            m_RecvBuffer = new byte[ushort.MaxValue / 2];
        }

        public bool RecvReliablePackets(ServerChannel channel, List<Packet> packets, IChannelListener listener, ServerHeartbeatProcessing heartbeat)
        {
            while (true)
            {
                int size = channel.Recv(m_RecvBuffer, 0, m_RecvBuffer.Length);

                if (size <= 0)
                {
                    break;
                }

                if (size == 8)
                {
                    ByteReadMemory memory = new ByteReadMemory(m_RecvBuffer, 0, size);
                    uint flag = memory.ReadUInt();
                    uint conv = memory.ReadUInt();

                    if (conv == channel.Conv)
                    {
                        if (flag == KcpConstants.Flag_Connect)
                        {
                            channel.SetConnectedStatus(true);
                            if (listener != null)
                            {
                                listener.OnAddChannel(channel);
                            }
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Disconnect)
                        {
                            channel.SetConnectedStatus(false);
                            if (listener != null)
                            {
                                listener.OnRemoveChannel(channel);
                            }
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Heartbeat)
                        {
                            heartbeat.UpdateHeartbeat(channel, m_RecvBuffer, 0, size);
                            continue;
                        }
                    }
                }

                PacketProcessing.Recv(m_RecvBuffer, 0, size, packets);
            }

            return packets.Count > 0;
        }

        public bool RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, List<Packet> packets)
        {
            PacketProcessing.Recv(rawBuffer, offset, count, packets);
            return packets.Count > 0;
        }

        public void SendPackets(ServerChannel channel, HConcurrentQueue<Packet> packets)
        {
            while (packets.TryDequeue(out Packet packet))
            {
                int size = PacketProcessing.Send(m_SendBuffer, packet);
                if (size > 0)
                {
                    if (packet.Channel == ChannelType.Reliable)
                    {
                        channel.Send(m_SendBuffer, 0, size);
                    }
                    else if (packet.Channel == ChannelType.Unreliable)
                    {
                        channel.RawSend(m_SendBuffer, 0, size);
                    }
                }
            }
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Nice.Game.Base
{
    internal class ClientDataProcessingCenter
    {
        private byte[] m_SendBuffer;
        private byte[] m_RecvBuffer;

        public ClientDataProcessingCenter()
        {
            m_SendBuffer = new byte[ushort.MaxValue / 2];
            m_RecvBuffer = new byte[ushort.MaxValue / 2];
        }

        public void RecvReliablePackets(ClientSocket socket, KcpCon kcp, List<Packet> packets, ClientHeartbeatProcessing heartbeat)
        {
            while (true)
            {
                int size = kcp.Recv(m_RecvBuffer, 0, m_RecvBuffer.Length);

                if (size <= 0)
                {
                    break;
                }

                if (size == 8)
                {
                    uint flag = KcpHelper.Decode32u(m_RecvBuffer, 0);
                    uint conv = KcpHelper.Decode32u(m_RecvBuffer, 4);

                    if (conv == kcp.Conv)
                    {
                        if (flag == KcpConstants.Flag_Heartbeat)
                        {
                            heartbeat.UpdateHeartbeat();
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Disconnect)
                        {
                            socket.Dispose();
                            continue;
                        }
                    }
                }

                PacketHandler.HandleRecv(m_RecvBuffer, 0, size, packets);
            }
        }

        public void RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, List<Packet> packets, ClientHeartbeatProcessing heartbeat)
        {
            heartbeat.UpdateHeartbeat();
            PacketHandler.HandleRecv(rawBuffer, offset, count, packets);
        }

        public void SendPackets(KcpCon kcp, ConcurrentQueue<Packet> packets)
        {
            while (packets.TryDequeue(out Packet packet))
            {
                int size = PacketHandler.HandleSend(m_SendBuffer, packet);
                if (size > 0)
                {
                    if (packet.Channel == MsgChannel.Reliable)
                    {
                        kcp.Send(m_SendBuffer, 0, size);
                    }
                    else if (packet.Channel == MsgChannel.Unreliable)
                    {
                        kcp.RawSend(m_SendBuffer, 0, size);
                    }
                }
            }
        }
    }

    internal class ServerDataProcessingCenter
    {
        private byte[] m_SendBuffer;
        private byte[] m_RecvBuffer;

        public ServerDataProcessingCenter()
        {
            m_SendBuffer = new byte[ushort.MaxValue / 2];
            m_RecvBuffer = new byte[ushort.MaxValue / 2];
        }

        public bool RecvReliablePackets(ServerChannel channel, List<Packet> packets, IKcpConnect connect, ServerHeartbeatProcessing heartbeat)
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
                    uint flag = KcpHelper.Decode32u(m_RecvBuffer, 0);
                    uint conv = KcpHelper.Decode32u(m_RecvBuffer, 4);

                    if (conv == channel.Conv)
                    {
                        if (flag == KcpConstants.Flag_Connect)
                        {
                            channel.SetConnectedStatus(true);
                            connect?.OnKcpConnect(channel);
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Disconnect)
                        {
                            channel.SetConnectedStatus(false);
                            connect?.OnKcpDisconnect(channel);
                            channel.Disconnect();
                            continue;
                        }

                        if (flag == KcpConstants.Flag_Heartbeat)
                        {
                            heartbeat.UpdateHeartbeat(channel, m_RecvBuffer, 0, 8);
                            continue;
                        }
                    }
                }

                PacketHandler.HandleRecv(m_RecvBuffer, 0, size, packets);
            }

            return packets.Count > 0;
        }

        public bool RecvUnreliablePackets(byte[] rawBuffer, int offset, int count, List<Packet> packets)
        {
            PacketHandler.HandleRecv(rawBuffer, offset, count, packets);
            return packets.Count > 0;
        }

        public void SendPackets(ServerChannel channel, ConcurrentQueue<Packet> packets)
        {
            while (packets.TryDequeue(out Packet packet))
            {
                int size = PacketHandler.HandleSend(m_SendBuffer, packet);
                if (size > 0)
                {
                    if (packet.Channel == MsgChannel.Reliable)
                    {
                        channel.Send(m_SendBuffer, 0, size);
                    }
                    else if (packet.Channel == MsgChannel.Unreliable)
                    {
                        channel.RawSend(m_SendBuffer, 0, size);
                    }
                }
            }
        }
    }

    internal static class PacketHandler
    {
        private const int CmdLength = 2;
        private const int MsgLength = 2;
        private const int CmdMsgLength = CmdLength + MsgLength;

        public static int HandleSend(byte[] buffer, Packet packet)
        {
            //消息格式
            //2字节消息类型，2字节消息长度，N字节消数据
            int size = CmdMsgLength + packet.Buffer.ReadableLength;

            if (size > buffer.Length)
            {
                Debug.LogError("PacketHandler HandleSend error total: " + size + " > buffer.Length:" + buffer.Length);
                return -1;
            }

            ByteWriteMemory memory = new ByteWriteMemory(buffer);

            //1.写入消息类型
            memory.Write(packet.Cmd);

            //2.写入消息长度
            memory.Write((ushort) packet.Buffer.ReadableLength);

            //3.写入消息数据
            memory.Write(packet.Buffer);

            return size;
        }


        public static void HandleRecv(byte[] data, int offset, int size, List<Packet> packets)
        {
            //消息格式
            //2字节消息类型，2字节消息长度，N字节消息数据
            while (size > CmdMsgLength)
            {
                //MsgLength个字节消息长度,CmdLength个字节消息类型长度,N字节消息数据长度
                ByteReadMemory memory = new ByteReadMemory(data, offset, size);

                //1.读取消息类型
                ushort cmd = memory.ReadUShort();
                size -= CmdLength;

                //2.读取消息长度
                ushort length = memory.ReadUShort();
                size -= MsgLength;

                if (length > size)
                {
                    Debug.LogError("PacketHandler HandleRecv error length:" + length + " > total:" + size);
                    break;
                }

                //3.读取消息数据
                ByteBuffer buffer = ByteBuffer.Allocate(length);
                memory.Read(buffer, length);
                size -= length;

                packets.Add(new Packet(cmd, buffer, 0));
            }
        }
    }
}
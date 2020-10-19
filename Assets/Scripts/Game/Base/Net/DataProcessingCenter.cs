using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zyq.Game.Base
{
    public class ClientDataProcessingCenter
    {
        private byte[] rawBuffer;
        private PacketHandler handler;

        public ClientDataProcessingCenter()
        {
            handler = new PacketHandler();
            rawBuffer = new byte[ushort.MaxValue];
        }

        public bool TryParseRecvKcpData(KcpConn con, List<Packet> packets)
        {
            int size = con.Recv(rawBuffer, 0, rawBuffer.Length);

            if (size > 0)
            {
                return handler.HandleRecv(rawBuffer, 0, size, packets);
            }

            return false;
        }

        public void TryParseSendKcpData(KcpConn con, ConcurrentQueue<Packet> sendPacketQueue)
        {
            while (sendPacketQueue.TryDequeue(out Packet packet))
            {
                int size = handler.HandleSend(rawBuffer, packet);
                if (size > 0)
                {
                    con.Send(rawBuffer, 0, size);
                }
            }
        }
    }

    public class ServerDataProcessingCenter
    {
        private byte[] rawBuffer;
        private PacketHandler handler;

        public ServerDataProcessingCenter()
        {
            handler = new PacketHandler();
            rawBuffer = new byte[ushort.MaxValue];
        }

        public bool TryParseRecvKcpData(ServerChannel channel, List<Packet> packets, IKcpConnect connect)
        {
            int size = channel.Recv(rawBuffer, 0, rawBuffer.Length);

            if (size == 8)
            {
                uint flag = KcpHelper.Decode32u(rawBuffer, 0);
                uint conv = KcpHelper.Decode32u(rawBuffer, 4);

                if (flag == KcpConstants.ConnectFlag && conv == channel.Conv)
                {
                    channel.SetConnectedStatus(true);
                    if (connect != null)
                    {
                        connect.OnKcpConnect(channel);
                    }

                    return false;
                }
                else if (flag == KcpConstants.DisconnectFlag && conv == channel.Conv)
                {
                    channel.SetConnectedStatus(false);
                    if (connect != null)
                    {
                        connect.OnKcpDisconnect(channel);
                    }

                    channel.Disconnect();
                    return false;
                }
            }

            if (size > 0)
            {
                return handler.HandleRecv(rawBuffer, 0, size, packets);
            }

            return false;
        }

        public void TryParseSendKcpData(ServerChannel channel, ConcurrentQueue<Packet> sendPacketQueue)
        {
            while (sendPacketQueue.TryDequeue(out Packet packet))
            {
                int size = handler.HandleSend(rawBuffer, packet);
                if (size > 0)
                {
                    channel.Send(rawBuffer, 0, size);
                }
            }
        }
    }

    public class PacketHandler
    {
        private const int MsgLength = 2;
        private const int CmdLength = 2;
        private const int MsgCmdLength = MsgLength + CmdLength;

        public int HandleSend(byte[] buffer, Packet packet)
        {
            //消息格式
            //MsgLength个字节消息长度,CmdLength个字节消息类型长度,N字节消息数据长度
            int total = MsgCmdLength + packet.Buffer.ReadableLength;

            if (total > buffer.Length)
            {
                Debug.LogError("PacketHandler HandleSend error total: " + total + " > buffer.Length:" + buffer.Length);
                return -1;
            }

            //消息长度 = 消息类型长度 + N字节消息数据长度
            int length = total - MsgLength;

            ByteWriteMemory memory = new ByteWriteMemory(buffer);

            //1.写入消息长度MsgLength字节
            memory.Write((ushort) length);

            //2.写入消息类型CmdLength字节
            memory.Write(packet.Cmd);

            //3.写入消息数据
            memory.Write(packet.Buffer);

            return total;
        }


        public bool HandleRecv(byte[] buffer, int offset, int total, List<Packet> packets)
        {
            while (total > MsgCmdLength)
            {
                //MsgLength个字节消息长度,CmdLength个字节消息类型长度,N字节消息数据长度
                ByteReadMemory memory = new ByteReadMemory(buffer, offset, total);

                //解析长度
                int length = memory.ReadUShort();

                total -= MsgLength;

                if (length > total)
                {
                    Debug.LogError("PacketHandler HandleRecv error length:" + length + " > total:" + total);
                    break;
                }

                total -= length;

                //读取CmdLength个字节消息类型
                ushort cmd = memory.ReadUShort();

                //(length - CmdLength)个字节的消息数据
                length -= CmdLength;
                ByteBuffer byteBuffer = ByteBuffer.Allocate(length);
                memory.Read(byteBuffer, length);

                packets.Add(new Packet(cmd, byteBuffer));
            }

            return packets.Count > 0;
        }
    }
}
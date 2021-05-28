using System;
using UnityEngine;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    internal static class PacketProcessing
    {
        private const int CmdLength = 2;
        private const int MsgLength = 2;
        private const int CmdMsgLength = CmdLength + MsgLength;

        public static int Send(byte[] buffer, Packet packet)
        {
            //消息格式
            //2字节消息类型，2字节消息长度，N字节消数据
            int size = CmdMsgLength + packet.Buffer.ReadableLength;

            if (size > buffer.Length)
            {
                throw new ArgumentException($"PacketProcessing Send error total: {size} > {buffer.Length}");
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

        public static void Recv(byte[] data, int offset, int size, List<Packet> packets)
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
                    throw new ArgumentException($"PacketProcessing Recv error: {length} > {size}");
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
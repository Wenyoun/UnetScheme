using System.IO;
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

        public bool TryRecvKcpData(KcpConn con, out Packet packet)
        {
            packet = new Packet();

            int size = con.Recv(rawBuffer, 0, rawBuffer.Length);
            if (size > 0)
            {
                packet = handler.HandleRecv(rawBuffer, 0, size);
                return true;
            }

            return false;
        }

        public void TrySendKcpData(KcpConn con, ConcurrentQueue<Packet> sendPacketQueue)
        {
            while (sendPacketQueue.TryDequeue(out Packet packet))
            {
                int size = handler.HandleSend(rawBuffer, packet);
                con.Send(rawBuffer, 0, size);
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

        public bool TryRecvKcpData(ServerChannel channel, out Packet packet, IKcpConnect kcpConnect)
        {
            packet = new Packet();
            
            int size = channel.Recv(rawBuffer, 0, rawBuffer.Length);
            if (size == 8)
            {
                uint flag = KcpHelper.Decode32u(rawBuffer, 0);
                uint conv = KcpHelper.Decode32u(rawBuffer, 4);

                if (flag == KcpHelper.ConnectFlag && conv == channel.Conv)
                {
                    channel.SetConnectedStatus(true);
                    if (kcpConnect != null)
                    {
                        kcpConnect.OnKcpConnect(channel);
                    }

                    return false;
                }
                else if (flag == KcpHelper.DisconnectFlag && conv == channel.Conv)
                {
                    channel.SetConnectedStatus(false);
                    if (kcpConnect != null)
                    {
                        kcpConnect.OnKcpDisconnect(channel);
                    }

                    channel.Disconnect();
                    return false;
                }
            }

            if (size > 0)
            {
                packet = handler.HandleRecv(rawBuffer, 0, size);
                return true;
            }

            return false;
        }

        public void TrySendKcpData(ServerChannel channel, ConcurrentQueue<Packet> sendPacketQueue)
        {
            while (sendPacketQueue.TryDequeue(out Packet packet))
            {
                int size = handler.HandleSend(rawBuffer, packet);
                channel.Send(rawBuffer, 0, size);
            }
        }
    }

    public class PacketHandler
    {
        private const int HeadLength = 2;

        public ushort ParseHeadLength(byte[] buffer, int offset)
        {
            return new ByteReadMemory(buffer, offset, HeadLength).ReadUShort();
        }

        public int HandleSend(byte[] buffer, Packet packet)
        {
            //消息格式
            //2个字节消息长度,2个字节消息类型长度,N字节消息数据长度
            int total = 4 + packet.Buffer.ReadableLength;

            if (total > buffer.Length)
            {
                throw new IOException("PacketHandler HandleSend total > buffer.Length");
            }

            //消息长度 = 消息类型长度 + N字节消息数据长度
            int length = total - 2;

            ByteWriteMemory memory = new ByteWriteMemory(buffer);

            //1.写入消息长度2字节
            memory.Write((ushort) length);

            //2.写入消息类型2字节
            memory.Write(packet.Cmd);

            //3.写入消息
            memory.Write(packet.Buffer);
            
            return total;
        }


        public Packet HandleRecv(byte[] buffer, int offset, int total)
        {
            //解析长度
            int length = ParseHeadLength(buffer, offset);
            
            if ((length + 2) > total)
            {
                throw new IOException("PacketHandler HandleRecv total > buffer.Length - offset");
            }
            
            offset += HeadLength;

            //消息格式
            //2个字节消息长度,2个字节消息类型长度,N字节消息数据长度
            ByteReadMemory memory = new ByteReadMemory(buffer, offset, length);

            //2个字节消息类型
            ushort cmd = memory.ReadUShort();
            length -= 2;

            //(length - 2)个字节的消息数据
            ByteBuffer byteBuffer = ByteBuffer.Allocate(length);
            memory.Read(byteBuffer, length);

            return new Packet(cmd, byteBuffer);
        }
    }
}
using System;
using System.IO;

namespace Zyq.Game.Base
{
    public struct ByteReadMemory
    {
        private int m_Count;
        private int m_Offset;
        private byte[] m_Buffer;

        public ByteReadMemory(byte[] buffer, int offset, int count)
        {
            if (offset < 0 || 
                offset >= buffer.Length || 
                count <= 0 || 
                count > buffer.Length ||
                buffer.Length - offset < count)
            {
                throw new IndexOutOfRangeException("ByteReadMemory exception buffer.length=" + buffer.Length +
                                                   ",offset=" + offset + ",count=" + count);
            }
            
            m_Count = count;
            m_Offset = offset;
            m_Buffer = buffer;
        }

        public void Read(ByteBuffer buffer, int length)
        {
            CheckCount(length);
            buffer.Write(m_Buffer, m_Offset, length);
            m_Offset += length;
            m_Count -= length;
        }

        public byte ReadByte()
        {
            CheckCount(1);
            byte value = m_Buffer[m_Offset++];
            m_Count -= 1;
            return value;
        }

        public bool ReadBool()
        {
            return ReadByte() > 0;
        }

        public short ReadShort()
        {
            CheckCount(2);
            short value = 0;
            value |= m_Buffer[m_Offset++];
            value |= (short) (m_Buffer[m_Offset++] << 8);
            m_Count -= 2;
            return value;
        }

        public ushort ReadUShort()
        {
            return (ushort) ReadShort();
        }

        public int ReadInt()
        {
            CheckCount(4);
            int value = 0;
            value |= m_Buffer[m_Offset++];
            value |= m_Buffer[m_Offset++] << 8;
            value |= m_Buffer[m_Offset++] << 16;
            value |= m_Buffer[m_Offset++] << 24;
            m_Count -= 4;
            return value;
        }

        public uint ReadUInt()
        {
            return (uint) ReadInt();
        }

        public long ReadLong()
        {
            CheckCount(8);
            long value = 0;
            value |= m_Buffer[m_Offset++];
            value |= (long) m_Buffer[m_Offset++] << 8;
            value |= (long) m_Buffer[m_Offset++] << 16;
            value |= (long) m_Buffer[m_Offset++] << 24;
            value |= (long) m_Buffer[m_Offset++] << 32;
            value |= (long) m_Buffer[m_Offset++] << 40;
            value |= (long) m_Buffer[m_Offset++] << 48;
            value |= (long) m_Buffer[m_Offset++] << 56;
            m_Count -= 8;
            return value;
        }

        public ulong ReadULong()
        {
            return (ulong) ReadLong();
        }

        public float ReadFloat()
        {
            UIntFloat converter = new UIntFloat
            {
                uintValue = ReadUInt()
            };
            return converter.floatValue;
        }

        public double ReadDouble()
        {
            ULongDouble converter = new ULongDouble
            {
                ulongValue = ReadULong()
            };
            return converter.doubleValue;
        }

        private void CheckCount(int count)
        {
            if (m_Count <= 0 || m_Count < count)
            {
                throw new EndOfStreamException("ByteReadMemory buffer out of range");
            }
        }
    }
}

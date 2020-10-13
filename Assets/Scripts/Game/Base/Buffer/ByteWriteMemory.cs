using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Zyq.Game.Base
{
    public struct ByteWriteMemory
    {
        private int m_Count;
        private int m_Offset;
        private byte[] m_Buffer;

        public ByteWriteMemory(byte[] buffer)
        {
            m_Offset = 0;
            m_Buffer = buffer;
            m_Count = buffer.Length;
        }

        public ByteWriteMemory(byte[] buffer, int offset, int count)
        {
            if (offset < 0 || 
                offset >= buffer.Length || 
                count <= 0 || 
                count > buffer.Length ||
                buffer.Length - offset < count)
            {
                throw new IndexOutOfRangeException("ByteWriteMemory exception buffer.length=" + buffer.Length +
                                                   ",offset=" + offset + ",count=" + count);
            }
            
            m_Count = count;
            m_Offset = offset;
            m_Buffer = buffer;
        }

        public void Write(ByteBuffer buffer)
        {
            int length = buffer.ReadableLength;
            CheckCount(length);
            buffer.Read(m_Buffer, m_Offset, length);
        }

        public void Write(byte value)
        {
            CheckCount(1);
            m_Buffer[m_Offset++] = value;
            m_Count -= 1;
        }

        public void Write(bool value)
        {
            Write(value ? 1 : 0);
        }

        public void Write(short value)
        {
            CheckCount(2);
            m_Buffer[m_Offset++] = (byte) value;
            m_Buffer[m_Offset++] = (byte) (value >> 8);
            m_Count -= 2;
        }

        public void Write(ushort value)
        {
            Write((short) value);
        }

        public void Write(int value)
        {
            CheckCount(4);
            m_Buffer[m_Offset++] = (byte) value;
            m_Buffer[m_Offset++] = (byte) (value >> 8);
            m_Buffer[m_Offset++] = (byte) (value >> 16);
            m_Buffer[m_Offset++] = (byte) (value >> 24);
            m_Count -= 4;
        }

        public void Write(uint value)
        {
            Write((int) value);
        }

        public void Write(long value)
        {
            CheckCount(8);
            m_Buffer[m_Offset++] = (byte) value;
            m_Buffer[m_Offset++] = (byte) (value >> 8);
            m_Buffer[m_Offset++] = (byte) (value >> 16);
            m_Buffer[m_Offset++] = (byte) (value >> 24);
            m_Buffer[m_Offset++] = (byte) (value >> 32);
            m_Buffer[m_Offset++] = (byte) (value >> 40);
            m_Buffer[m_Offset++] = (byte) (value >> 48);
            m_Buffer[m_Offset++] = (byte) (value >> 56);
            m_Count -= 8;
        }

        public void Write(ulong value)
        {
            Write((long) value);
        }

        public void Write(float value)
        {
            UIntFloat convert = new UIntFloat
            {
                floatValue = value
            };
            Write(convert.uintValue);
        }

        public void Write(double value)
        {
            ULongDouble convert = new ULongDouble
            {
                doubleValue = value
            };
            Write(convert.ulongValue);
        }
        
        private void CheckCount(int count)
        {
            if (m_Count < count || m_Count <= 0)
            {
                throw new EndOfStreamException("ByteWriteMemory buffer out of range");
            }
        }
    }
}
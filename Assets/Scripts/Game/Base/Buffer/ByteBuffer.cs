using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Zyq.Game.Base
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)] public float floatValue;

        [FieldOffset(0)] public uint uintValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct ULongDouble
    {
        [FieldOffset(0)] public double doubleValue;

        [FieldOffset(0)] public ulong ulongValue;
    }

    public class ByteBuffer : IDisposable
    {
        private const int MaxStringLength = short.MaxValue;

        private static readonly UTF8Encoding ENCODING = new UTF8Encoding(false, true);

        private static readonly ConcurrentQueue<ByteBuffer> POOL = new ConcurrentQueue<ByteBuffer>();

        public static ByteBuffer Allocate(int maxCapacity)
        {
            ByteBuffer byteBuffer;

            if (!POOL.TryDequeue(out byteBuffer))
            {
                byteBuffer = new ByteBuffer(maxCapacity);
            }

            return byteBuffer;
        }

        private int readIndex;

        private int writeIndex;

        private int maxCapacity;

        private byte[] rawBuffer;

        private byte[] stringBuffer;

        private ByteBuffer(int maxCapacity)
        {
            this.readIndex = 0;
            this.writeIndex = 0;
            this.maxCapacity = maxCapacity;
            this.rawBuffer = new byte[maxCapacity];
        }

        #region Write

        public void Write(ByteBuffer buffer)
        {
            int length = buffer.ReadableLength;
            EnsureLength(length);
            Array.Copy(buffer.rawBuffer, buffer.readIndex, rawBuffer, writeIndex, length);
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            EnsureLength(length);
            Array.Copy(buffer, offset, rawBuffer, writeIndex, length);
            writeIndex += length;
        }

        public void Write(byte value)
        {
            EnsureLength(1);
            rawBuffer[writeIndex++] = value;
        }

        public void Write(bool value)
        {
            Write(value ? 1 : 0);
        }

        public void Write(short value)
        {
            EnsureLength(2);
            rawBuffer[writeIndex++] = (byte) value;
            rawBuffer[writeIndex++] = (byte) (value >> 8);
        }

        public void Write(ushort value)
        {
            Write((short) value);
        }

        public void Write(int value)
        {
            EnsureLength(4);
            rawBuffer[writeIndex++] = (byte) value;
            rawBuffer[writeIndex++] = (byte) (value >> 8);
            rawBuffer[writeIndex++] = (byte) (value >> 16);
            rawBuffer[writeIndex++] = (byte) (value >> 24);
        }

        public void Write(uint value)
        {
            Write((int) value);
        }

        public void Write(long value)
        {
            EnsureLength(8);
            rawBuffer[writeIndex++] = (byte) value;
            rawBuffer[writeIndex++] = (byte) (value >> 8);
            rawBuffer[writeIndex++] = (byte) (value >> 16);
            rawBuffer[writeIndex++] = (byte) (value >> 24);
            rawBuffer[writeIndex++] = (byte) (value >> 32);
            rawBuffer[writeIndex++] = (byte) (value >> 40);
            rawBuffer[writeIndex++] = (byte) (value >> 48);
            rawBuffer[writeIndex++] = (byte) (value >> 56);
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

        public void Write(string value)
        {
            if (stringBuffer == null)
            {
                stringBuffer = new byte[MaxStringLength];
            }

            if (value.Length > MaxStringLength)
            {
                throw new IndexOutOfRangeException("ByteBuffer.Write(string) too long: " + value.Length + ". Limit: " +
                                                   MaxStringLength);
            }

            int size = ENCODING.GetBytes(value, 0, value.Length, stringBuffer, 0);

            Write((short) size);
            Write(stringBuffer, 0, size);
        }

        #endregion

        #region Read

        public void Read(byte[] buffer, int offset, int length)
        {
            length = Math.Min(length, ReadableLength);
            CheckLength(length);
            Array.Copy(rawBuffer, readIndex, buffer, offset, length);
            readIndex += length;
        }

        public byte ReadByte()
        {
            CheckLength(1);
            return rawBuffer[readIndex++];
        }

        public bool ReadBool()
        {
            return ReadByte() > 0;
        }

        public short ReadShort()
        {
            CheckLength(2);
            short value = 0;
            value |= rawBuffer[readIndex++];
            value |= (short) (rawBuffer[readIndex++] << 8);
            return value;
        }

        public ushort ReadUShort()
        {
            return (ushort) ReadShort();
        }

        public int ReadInt()
        {
            CheckLength(4);
            int value = 0;
            value |= rawBuffer[readIndex++];
            value |= rawBuffer[readIndex++] << 8;
            value |= rawBuffer[readIndex++] << 16;
            value |= rawBuffer[readIndex++] << 24;
            return value;
        }

        public uint ReadUInt()
        {
            return (uint) ReadInt();
        }

        public long ReadLong()
        {
            CheckLength(8);
            long value = 0;
            value |= rawBuffer[readIndex++];
            value |= (long) rawBuffer[readIndex++] << 8;
            value |= (long) rawBuffer[readIndex++] << 16;
            value |= (long) rawBuffer[readIndex++] << 24;
            value |= (long) rawBuffer[readIndex++] << 32;
            value |= (long) rawBuffer[readIndex++] << 40;
            value |= (long) rawBuffer[readIndex++] << 48;
            value |= (long) rawBuffer[readIndex++] << 56;
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

        public string ReadString()
        {
            ushort length = ReadUShort();

            if (length == 0)
            {
                return null;
            }

            if (length > MaxStringLength)
            {
                throw new IndexOutOfRangeException("ByteBuffer.ReadString() too long: " + length + ". Limit: " +
                                                   MaxStringLength);
            }

            CheckLength(length);

            ArraySegment<byte> data = new ArraySegment<byte>(rawBuffer, readIndex, length);

            readIndex += length;

            return ENCODING.GetString(data.Array, data.Offset, data.Count);
        }

        #endregion

        #region properties

        public int ReadIndex => readIndex;

        public int WriteIndex => writeIndex;
        
        public byte[] RawBuffer => rawBuffer;

        public int ReadableLength => writeIndex - readIndex;

        public int WritableLength => maxCapacity - writeIndex;

        #endregion

        public void Reset()
        {
            readIndex = 0;
            writeIndex = 0;
        }

        public void Dispose()
        {
            Reset();
            POOL.Enqueue(this);
        }

        private void CheckLength(int length)
        {
            int readableLength = ReadableLength;
            if (length > readableLength)
            {
                throw new EndOfStreamException("raw buffer out of range current=" + readableLength + ",length=" +
                                               length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureLength(int length)
        {
            int writableLength = WritableLength;
            if (length > writableLength)
            {
                EnsureCapacity(length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int capacity)
        {
            if (maxCapacity < capacity)
            {
                maxCapacity = Math.Max(capacity, maxCapacity * 2);
                Array.Resize(ref rawBuffer, maxCapacity);
            }
        }
    }
}
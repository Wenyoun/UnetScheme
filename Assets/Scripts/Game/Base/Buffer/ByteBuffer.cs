using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Nice.Game.Base
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;

        [FieldOffset(0)]
        public uint uintValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct ULongDouble
    {
        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong ulongValue;
    }

    public class ByteBuffer : IDisposable
    {
        private const int MaxStringLength = short.MaxValue;
        private static readonly UTF8Encoding Encoding = new UTF8Encoding(false, true);
        private static readonly ConcurrentQueue<ByteBuffer> Pool = new ConcurrentQueue<ByteBuffer>();

        public static ByteBuffer Allocate(int maxCapacity)
        {
            ByteBuffer byteBuffer;

            if (!Pool.TryDequeue(out byteBuffer))
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

        private ByteBuffer(int capacity)
        {
            readIndex = 0;
            writeIndex = 0;
            maxCapacity = capacity;
            rawBuffer = new byte[capacity];
        }

        #region Write
        public void Write(ByteBuffer input)
        {
            Write(input, input.ReadableLength);
        }

        public void Write(ByteBuffer input, int length)
        {
            input.CheckLength(length);
            Write(input.rawBuffer, input.readIndex, length);
            input.readIndex += length;
        }

        public void Write(byte[] input, int offset, int length)
        {
            EnsureLength(length);
            Array.Copy(input, offset, rawBuffer, writeIndex, length);
            writeIndex += length;
        }

        public void Write(byte value)
        {
            EnsureLength(1);
            rawBuffer[writeIndex++] = value;
        }

        public void Write(bool value)
        {
            Write((byte) (value ? 1 : 0));
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
            UIntFloat convert = new UIntFloat {floatValue = value};
            Write(convert.uintValue);
        }

        public void Write(double value)
        {
            ULongDouble convert = new ULongDouble {doubleValue = value};
            Write(convert.ulongValue);
        }

        public void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Write((short) 0);
                return;
            }

            if (stringBuffer == null)
            {
                stringBuffer = new byte[MaxStringLength];
            }

            if (value.Length > MaxStringLength)
            {
                throw new IndexOutOfRangeException("ByteBuffer.Write(string) too long: " + value.Length + ". Limit: " + MaxStringLength);
            }

            int size = Encoding.GetBytes(value, 0, value.Length, stringBuffer, 0);
            Write((short) size);
            Write(stringBuffer, 0, size);
        }

        public void Write(Vector2 value)
        {
            EnsureLength(8);
            Write(value.x);
            Write(value.y);
        }

        public void Write(Vector3 value)
        {
            EnsureLength(12);
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        public void Write(Vector4 value)
        {
            EnsureLength(16);
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        public void Write(Quaternion value)
        {
            EnsureLength(16);
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
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

        public bool ReadBoolean()
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
            UIntFloat converter = new UIntFloat {uintValue = ReadUInt()};
            return converter.floatValue;
        }

        public double ReadDouble()
        {
            ULongDouble converter = new ULongDouble {ulongValue = ReadULong()};
            return converter.doubleValue;
        }

        public string ReadString()
        {
            ushort length = ReadUShort();

            if (length == 0)
            {
                return "";
            }

            if (length > MaxStringLength)
            {
                throw new IndexOutOfRangeException("ByteBuffer.ReadString() too long: " + length + ". Limit: " + MaxStringLength);
            }

            CheckLength(length);

            ArraySegment<byte> data = new ArraySegment<byte>(rawBuffer, readIndex, length);

            readIndex += length;

            return Encoding.GetString(data.Array, data.Offset, data.Count);
        }

        public Vector2 ReadVector2()
        {
            Vector2 value = new Vector2();
            value.x = ReadFloat();
            value.y = ReadFloat();
            return value;
        }

        public Vector3 ReadVector3()
        {
            Vector3 value = new Vector3();
            value.x = ReadFloat();
            value.y = ReadFloat();
            value.z = ReadFloat();
            return value;
        }

        public Vector4 ReadVector4()
        {
            Vector4 value = new Vector4();
            value.x = ReadFloat();
            value.y = ReadFloat();
            value.z = ReadFloat();
            value.w = ReadFloat();
            return value;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion value = new Quaternion();
            value.x = ReadFloat();
            value.y = ReadFloat();
            value.z = ReadFloat();
            value.w = ReadFloat();
            return value;
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
            Pool.Enqueue(this);
        }

        private void CheckLength(int length)
        {
            int readableLength = ReadableLength;
            if (length > readableLength)
            {
                throw new EndOfStreamException("raw buffer out of range current readable length=" + readableLength + ",ready length=" + length);
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
            capacity = writeIndex + capacity;
            if (maxCapacity < capacity)
            {
                maxCapacity = Math.Max(capacity, maxCapacity * 2);
                Array.Resize(ref rawBuffer, maxCapacity);
            }
        }
    }
}
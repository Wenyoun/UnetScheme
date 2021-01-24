namespace Nice.Game.Base
{
    public static class ByteBufferUtils
    {
        private static readonly ByteBuffer Empty = ByteBuffer.Allocate(0);

        public static void Write(ByteBuffer src, ByteBuffer dst)
        {
            short length = (short) (null != src ? src.ReadableLength : 0);
            dst.Write(length);
            if (length > 0)
            {
                dst.Write(src);
            }
        }

        public static ByteBuffer Read(ByteBuffer input)
        {
            int length = input.ReadShort();
            if (length > 0)
            {
                ByteBuffer buffer = ByteBuffer.Allocate(512);
                buffer.Write(input, length);
                return buffer;
            }
            return Empty;
        }
    }
}
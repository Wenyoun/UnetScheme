namespace Nice.Game.Base
{
    public static class ByteUtils
    {
        private static readonly byte[] Empty = new byte[0];

        public static void Write(byte[] src, ByteBuffer dst)
        {
            short length = (short) (null != src ? src.Length : 0);
            dst.Write(length);
            if (length > 0)
            {
                dst.Write(src, 0, length);
            }
        }

        public static byte[] Read(ByteBuffer input)
        {
            int length = input.ReadShort();
            if (length > 0)
            {
                byte[] data = new byte[length];
                input.Read(data, 0, length);
                return data;
            }
            return Empty;
        }
    }
}
namespace Nice.Game.Base
{
    public struct Packet
    {
        public ushort Cmd;
        public byte Channel;
        public ByteBuffer Buffer;

        public Packet(ushort cmd, ByteBuffer buffer, byte channel)
        {
            Cmd = cmd;
            Buffer = buffer;
            Channel = channel;
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            Buffer.Read(buffer, offset, length);
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            Buffer.Write(buffer, offset, length);
        }
    }
}
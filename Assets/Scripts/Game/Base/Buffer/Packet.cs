namespace Zyq.Game.Base
{
    public struct Packet
    {
        public ushort Cmd;

        public ByteBuffer Buffer;

        public Packet(ushort cmd, ByteBuffer buffer)
        {
            Cmd = cmd;
            Buffer = buffer;
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
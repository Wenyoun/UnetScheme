namespace Nice.Game.Base {
    internal struct Packet {
        public ushort Cmd;
        public ByteBuffer Buffer;
        public ChannelType Channel;

        public Packet(ushort cmd, ByteBuffer buffer, ChannelType channel) {
            Cmd = cmd;
            Buffer = buffer;
            Channel = channel;
        }

        public void Read(byte[] buffer, int offset, int length) {
            Buffer.Read(buffer, offset, length);
        }

        public void Write(byte[] buffer, int offset, int length) {
            Buffer.Write(buffer, offset, length);
        }
    }
}
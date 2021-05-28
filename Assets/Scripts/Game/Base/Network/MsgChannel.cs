
namespace Nice.Game.Base
{
    public enum ChannelType
    {
        Reliable = 1,
        Unreliable = 2
    }

    internal static class MsgChannel
    {
        public const byte Reliable = 0x01;
        public const byte Unreliable = 0x02;
    }
}
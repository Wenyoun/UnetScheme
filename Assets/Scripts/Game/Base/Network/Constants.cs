namespace Nice.Game.Base
{
    internal class HeartbaetConstants
    {
        public const int Send_Interval_Mills = 10000;
        public const int Timeout_Interval_Mills = 30000;
    }

    internal class KcpConstants
    {
        public const int Conv_Size = 4;
        public const int Head_Size = Conv_Size + 1;
        public const int Packet_Length = 1300;
        public const uint Flag_Connect = 0x0ffffff0;
        public const uint Flag_Heartbeat = 0x0ffffff1;
        public const uint Flag_Disconnect = 0x0fffffff;
    }
}
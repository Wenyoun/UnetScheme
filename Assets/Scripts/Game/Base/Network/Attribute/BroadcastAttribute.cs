using System;

namespace Nice.Game.Base
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BroadcastAttribute : Attribute
    {
        public ushort MsgId;
        public ChannelType Channel;

        public BroadcastAttribute(ushort msgId, ChannelType channel = ChannelType.Reliable)
        {
            MsgId = msgId;
            Channel = channel;
        }
    }
}
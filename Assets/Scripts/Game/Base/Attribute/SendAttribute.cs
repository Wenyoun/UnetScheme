using System;

namespace Nice.Game.Base {
    [AttributeUsage(AttributeTargets.Method)]
    public class SendAttribute : Attribute {
        public ushort MsgId;
        public ChannelType Channel;

        public SendAttribute(ushort msgId, ChannelType channel = ChannelType.Reliable) {
            MsgId = msgId;
            Channel = channel;
        }
    }
}
using System;

namespace Nice.Game.Base {
    [AttributeUsage(AttributeTargets.Method)]
    public class SendAttribute : Attribute {
        public ushort MsgId;
        public byte Channel;

        public SendAttribute(ushort msgId, byte channel = MsgChannel.Reliable) {
            MsgId = msgId;
            Channel = channel;
        }
    }
}
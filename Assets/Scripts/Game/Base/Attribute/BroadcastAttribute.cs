using System;

namespace Nice.Game.Base {
    [AttributeUsage(AttributeTargets.Method)]
    public class BroadcastAttribute : Attribute {
        public ushort MsgId;
        public byte Channel;

        public BroadcastAttribute(ushort msgId, byte channel = MsgChannel.Reliable) {
            MsgId = msgId;
            Channel = channel;
        }
    }
}
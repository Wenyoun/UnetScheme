using System;

namespace Zyq.Game.Base {
    [AttributeUsage(AttributeTargets.Method)]
    public class BroadcastAttribute : Attribute {
        public short MsgId;

        public BroadcastAttribute(short msgId) {
            MsgId = msgId;
        }
    }
}
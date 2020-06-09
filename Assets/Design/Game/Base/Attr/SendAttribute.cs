using System;

namespace Zyq.Game.Base {
    [AttributeUsage(AttributeTargets.Method)]
    public class SendAttribute : Attribute {
        public short MsgId;

        public SendAttribute(short msgId) {
            MsgId = msgId;
        }
    }
}
using System;

namespace Zyq.Game.Base {
    [AttributeUsage(AttributeTargets.Method)]
    public class RecvAttribute : Attribute {
        public short MsgId;

        public RecvAttribute(short msgId) {
            MsgId = msgId;
        }
    }
}
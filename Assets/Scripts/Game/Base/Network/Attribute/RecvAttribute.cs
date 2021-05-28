using System;

namespace Nice.Game.Base
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RecvAttribute : Attribute
    {
        public ushort MsgId;

        public RecvAttribute(ushort msgId)
        {
            MsgId = msgId;
        }
    }
}
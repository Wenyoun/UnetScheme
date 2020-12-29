using System;

namespace Nice.Game.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BroadcastAttribute : Attribute
    {
        public ushort MsgId;

        public BroadcastAttribute(ushort msgId)
        {
            MsgId = msgId;
        }
    }
}
using System;

namespace Nice.Game.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SendAttribute : Attribute
    {
        public ushort MsgId;

        public SendAttribute(ushort msgId)
        {
            MsgId = msgId;
        }
    }
}
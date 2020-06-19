using System;

namespace Zyq.Game.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SendAttribute : Attribute
    {
        public short MsgId;

        public SendAttribute(short msgId)
        {
            MsgId = msgId;
        }
    }
}
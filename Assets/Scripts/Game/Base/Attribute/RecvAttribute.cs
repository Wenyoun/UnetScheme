using System;

namespace Zyq.Game.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RecvAttribute : Attribute
    {
        public short MsgId;

        public RecvAttribute(short msgId)
        {
            MsgId = msgId;
        }
    }
}
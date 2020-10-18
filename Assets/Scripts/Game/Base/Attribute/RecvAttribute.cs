using System;

namespace Zyq.Game.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RecvAttribute : Attribute
    {
        public ushort MsgId;

        public RecvAttribute(ushort msgId)
        {
            MsgId = msgId;
        }
    }
}
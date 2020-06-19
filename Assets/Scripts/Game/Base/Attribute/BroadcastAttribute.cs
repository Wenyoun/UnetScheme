using System;

namespace Zyq.Game.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BroadcastAttribute : Attribute
    {
        public short MsgId;

        public BroadcastAttribute(short msgId)
        {
            MsgId = msgId;
        }
    }
}
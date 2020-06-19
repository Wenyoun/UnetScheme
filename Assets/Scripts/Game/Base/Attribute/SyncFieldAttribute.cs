using System;

namespace Zyq.Game.Base
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SyncFieldAttribute : Attribute
    {
    }
}
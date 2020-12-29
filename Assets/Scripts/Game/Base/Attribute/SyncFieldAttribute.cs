using System;

namespace Nice.Game.Base
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SyncFieldAttribute : Attribute
    {
    }
}
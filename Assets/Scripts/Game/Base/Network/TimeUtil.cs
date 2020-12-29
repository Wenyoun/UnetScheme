using System;

namespace Nice.Game.Base
{
    internal static class TimeUtil
    {
        /* 获取 utc 1970-1-1到现在的秒数 */
        public static long Get1970ToNowSeconds()
        {
            return (DateTime.UtcNow.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /* 获取 utc 1970-1-1到现在的毫秒数 */
        public static long Get1970ToNowMilliseconds()
        {
            return (DateTime.UtcNow.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
    }
}
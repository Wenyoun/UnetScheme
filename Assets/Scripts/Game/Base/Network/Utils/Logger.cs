namespace Nice.Game.Base
{
    internal static class Logger
    {
        public static void Debug(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }
    }
}
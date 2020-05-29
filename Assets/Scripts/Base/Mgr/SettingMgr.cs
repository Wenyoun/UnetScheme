using UnityEngine;

namespace Base
{
    public sealed class SettingMgr
    {
        public static void Config()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }
    }
}

namespace Game
{
    //战斗管理器
    public class BattleMgr
    {
        public static void Init()
        {
            CachedMgr.Init();
            EntityMgr.Init();
        }

        public static void Dispose()
        {
            EntityMgr.Dispose();
            CachedMgr.Dispose();
        }
    }
}
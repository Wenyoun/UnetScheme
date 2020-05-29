namespace Game
{
    //缓存管理
    public class CachedMgr
    {
        //特效池
        public static FxCachedPool Fx;

        public static void Init()
        {
            Fx = new FxCachedPool();
            Fx.OnInit();
        }

        public static void Dispose()
        {
            Fx.OnRemove();
            Fx = null;
        }
    }
}
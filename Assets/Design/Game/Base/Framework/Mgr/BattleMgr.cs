namespace Zyq.Game.Base {
    //战斗管理器
    public class BattleMgr {
        public static void Init() {
            EntityMgr.Init();
        }

        public static void Dispose() {
            EntityMgr.Dispose();
        }
    }
}
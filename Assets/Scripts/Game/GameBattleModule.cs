using Base;
using Zyq.Game.Base;

namespace Game {
    public class GameBattleModule : AbsCompose {
        public override void OnInit() {
            UpdateMgr.Init();
            MessageMgr.Init();
            TimerMgr.Init();
            BattleMgr.Init();
        }

        public override void OnRemove() {
            TimerMgr.Dispose();
            BattleMgr.Dispose();
            MessageMgr.Dipose();
            UpdateMgr.Dispose();
        }

        public override void OnUpdate(float delta) {
            UpdateMgr.OnUpdate(delta);
        }

        public override void OnFixedUpdate(float delta) {
            UpdateMgr.OnFixedUpdate(delta);
        }
    }
}
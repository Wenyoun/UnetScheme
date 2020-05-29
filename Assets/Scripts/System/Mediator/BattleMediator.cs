using Base;

namespace System
{
    public class BattleMediator : AbsViewMediator
    {
        public BattleMediator(AbsViewModule module, bool now = false)
           : base("BattleMediator", SystemDisplayConstants.Show_Battle_View, SystemDisplayConstants.Hide_Battle_View, module, now)
        {
        }

        public override AbsView CreateView(AbsViewModule module)
        {
            return new BattleView(SimpleResMgr.CreateViewRoot("Prefabs/View/BattleView", module.NRoot.transform));
        }
    }
}
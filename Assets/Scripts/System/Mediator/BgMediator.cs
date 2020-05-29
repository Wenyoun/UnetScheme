using Base;

namespace System
{
    public class BgMediator : AbsViewMediator
    {
        public BgMediator(AbsViewModule module, bool now = false)
           : base("BgMediator", SystemDisplayConstants.Show_Bg_View, SystemDisplayConstants.Hide_Bg_View, module, now)
        {
        }

        public override AbsView CreateView(AbsViewModule module)
        {
            return new BgView(SimpleResMgr.CreateViewRoot("Prefabs/View/BgView", module.BRoot.transform));
        }
    }
}
using Base;

namespace System
{
    public class ResultMediator : AbsViewMediator
    {
        public ResultMediator(AbsViewModule module, bool now = false)
           : base("ResultMediator", SystemDisplayConstants.Show_Result_View, SystemDisplayConstants.Hide_Result_View, module, now)
        {
        }

        public override AbsView CreateView(AbsViewModule module)
        {
            return new ResultView(SimpleResMgr.CreateViewRoot("Prefabs/View/ResultView", module.NRoot.transform));
        }
    }
}
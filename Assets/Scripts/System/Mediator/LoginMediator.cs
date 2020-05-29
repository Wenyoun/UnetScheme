using Base;

namespace System
{
    public class LoginMediator : AbsViewMediator
    {
        public LoginMediator(AbsViewModule module, bool now = false)
           : base("LoginMediator", SystemDisplayConstants.Show_Login_View, SystemDisplayConstants.Hide_Login_View, module, now)
        {
        }

        public override AbsView CreateView(AbsViewModule module)
        {
            return new LoginView(SimpleResMgr.CreateViewRoot("Prefabs/View/LoginView", module.NRoot.transform));
        }
    }
}
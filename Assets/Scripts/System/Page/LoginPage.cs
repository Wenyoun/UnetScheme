using Base;

namespace System
{
    public class LoginPage : IPage
    {
        public void Show(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Show_Bg_View);
            Facade.Ins.Notify(SystemDisplayConstants.Show_Login_View);
        }

        public void Hide(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Bg_View);
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Login_View);
        }
    }
}
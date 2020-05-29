using Base;
using Game;

namespace System
{
    public class BattlePage : IPage
    {
        public void Show(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Show_Bg_View);
            Facade.Ins.Notify(SystemDisplayConstants.Show_Result_View, "服务器运行中...");
            Facade.Ins.Notify(SystemDisplayConstants.Show_Battle_View);
        }

        public void Hide(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Bg_View);
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Result_View);
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Battle_View);
        }
    }
}
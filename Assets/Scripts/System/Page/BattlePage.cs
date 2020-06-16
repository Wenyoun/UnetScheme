using Base;

namespace System
{
    public class BattlePage : IPage
    {
        public void Show(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Show_Battle_View);
        }

        public void Hide(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Battle_View);
        }
    }
}
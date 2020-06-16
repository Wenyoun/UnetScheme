using Base;

namespace System
{
    public class LobbyPage : IPage
    {
        public void Show(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Show_Bg_View);
            Facade.Ins.Notify(SystemDisplayConstants.Show_Lobby_View);
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Result_View);
        }

        public void Hide(object body)
        {
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Bg_View);
            Facade.Ins.Notify(SystemDisplayConstants.Hide_Lobby_View);
        }
    }
}
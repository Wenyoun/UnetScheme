using Base;

namespace System
{
    public class LobbyMediator : AbsViewMediator
    {
        public LobbyMediator(AbsViewModule module, bool now = false)
           : base("LobbyMediator", SystemDisplayConstants.Show_Lobby_View, SystemDisplayConstants.Hide_Lobby_View, module, now)
        {
        }

        public override AbsView CreateView(AbsViewModule module)
        {
            return new LobbyView(SimpleResMgr.CreateViewRoot("Prefabs/View/LobbyView", module.NRoot.transform));
        }
    }
}
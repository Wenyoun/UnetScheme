using Base;
using UnityEngine;

namespace System
{
    public class SystemViewModule : AbsViewModule
    {
        public override GameObject CreateRoot()
        {
            GameObject root = SimpleResMgr.CreateRoot("Prefabs/View/System", GlobalMgr.Root.transform);
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.worldCamera = GlobalMgr.UI;
            return root;
        }

        public override void OnInit()
        {
            base.OnInit();
            PageMgr.Ins.Push<LoginPage>();
        }

        public override void RegMediators()
        {
            RegMediator(new BgMediator(this));
            RegMediator(new LoginMediator(this));
            RegMediator(new LobbyMediator(this));
            RegMediator(new BattleMediator(this));
            RegMediator(new ResultMediator(this));
        }
    }
}
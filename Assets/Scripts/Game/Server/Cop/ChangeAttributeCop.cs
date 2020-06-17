using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class ChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            Server.Ins.TimerMgr.Register(5, () =>
            {
                attribute.Hp1 = 10;
                attribute.Hp11 = "yinhuyaong999";
            });
        }
    }
}
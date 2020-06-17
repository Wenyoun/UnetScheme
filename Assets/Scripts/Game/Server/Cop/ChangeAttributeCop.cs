using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class ChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            for (int i = 5; i < 10; ++i)
            {
                int k = i;
                Server.Ins.TimerMgr.Register(i, () =>
                {
                    attribute.Hp1 = k;
                    attribute.Hp11 = "yinhuyaong999" + k;
                });
            }
        }
    }
}
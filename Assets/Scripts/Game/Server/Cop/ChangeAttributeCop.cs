using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class ChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            ConnectionFeture connection = Entity.GetFeture<ConnectionFeture>();
            for (int i = 1; i < 10; ++i)
            {
                int k = i;
                Server.Ins.TimerMgr.Register(i, () =>
                {
                    attribute.Hp1 = k;
                    attribute.Hp11 = "yinhuyaong->" + k;
                    attribute.Hp12 = new Vector3(k, k, k);
                });
            }
        }
    }
}
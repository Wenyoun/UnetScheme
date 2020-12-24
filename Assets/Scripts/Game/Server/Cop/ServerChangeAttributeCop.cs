using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class ServerChangeAttributeCop : AbsCop
    {
        protected override void Init()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            ConnectionFeture connection = Entity.GetFeture<ConnectionFeture>();
            for (int i = 1; i < 100; ++i)
            {
                int k = i;
                Entity.Timer.Register(i, () => { attribute.Hp1 = k; });
            }
        }
    }
}
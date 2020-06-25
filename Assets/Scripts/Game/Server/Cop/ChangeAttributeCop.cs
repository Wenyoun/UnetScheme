using Zyq.Game.Base;
using UnityEngine.Networking;

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
                    RpcNotify("yinhuayong");
                });
            }
        }

        private void RpcNotify(string username)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(1);
            writer.Write(1);
            writer.Write(Entity.Eid);
            writer.Write(username);
            ConnectionFeture target = Entity.GetFeture<ConnectionFeture>();
            if (target != null)
            {
                target.Send(writer);
            }
        }

        [Send(NetMsgId.Msg_Create_Player2)]
        private void Test(Connection target, string usename)
        {
        }
    }
}
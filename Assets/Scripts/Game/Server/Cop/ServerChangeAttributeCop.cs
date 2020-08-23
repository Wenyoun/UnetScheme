using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Server
{
    public class ServerChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            ConnectionFeture connection = Entity.GetFeture<ConnectionFeture>();
            for (int i = 1; i < 100; ++i)
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

        [Recv(NetMsgId.Msg_Create_Player1)]
        public void OnRecvArray(int v1, int[] v2, LoginData[] v4)
        {
        }
    }
}
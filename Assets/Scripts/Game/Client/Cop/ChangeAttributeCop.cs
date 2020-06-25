using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class ChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            RegisterMessage(MessageConstants.Sync_Attribute, (IBody body) =>
            {
                UnityEngine.Debug.Log(attribute.Hp1 + "." + attribute.Hp11);
            });
        }

        [Recv(NetMsgId.Msg_Test)]
        public void TestRecv(string username)
        {
            Debug.Log(username);
        }
    }
}
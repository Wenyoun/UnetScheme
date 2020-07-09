using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class ChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            RegisterMessage(MessageConstants.Sync_Attribute, (IBody body) =>
            {
                UnityEngine.Debug.Log(attribute.Hp1 + "." + attribute.Hp11 + "," + attribute.Hp12);
            });
        }

        [Recv(NetMsgId.Msg_Create_Player2)]
        private void Recv(int v1, Test1 v2, int v3, string v4, Test1 v5)
        {
        }
    }
}
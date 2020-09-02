using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Client
{
    public class ChangeAttributeCop : AbsCop
    {
        public override void OnInit()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            RegisterMessage(MessageConstants.Sync_Attribute, (IBody body) =>
            {
                //UnityEngine.Debug.Log(attribute.Hp1 + "." + attribute.Hp11 + "," + attribute.Hp12);
            });
        }

        [Recv(NetMsgId.Msg_Create_Player1)]
        public void OnRecvArray(int v1, int[] v2, LoginData[] v4)
        {
        }
    }
}
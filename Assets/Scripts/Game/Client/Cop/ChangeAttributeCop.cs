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
                UnityEngine.Debug.Log(attribute.Hp1 + ":" + attribute.Hp11);
            });
        }


        [Recv(NetMsgId.Msg_Create_Player2)]
        public void RpcLogin(byte v1,
                                   bool v2,
                                   short v3,
                                   int v4,
                                   long v5,
                                   ushort v6,
                                   uint v7,
                                   ulong v8,
                                   float v9,
                                   double v10,
                                   string v11,
                                   Vector2 v12,
                                   Vector3 v13,
                                   Vector4 v14,
                                   Quaternion v15)
        {
        }
    }
}
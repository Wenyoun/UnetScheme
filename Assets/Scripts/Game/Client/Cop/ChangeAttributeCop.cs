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
                UnityEngine.Debug.Log(attribute.Hp1 + ":" + attribute.Hp11);
            });
            //OnRegisterChangeAttributeCopHandler();
        }
        public override void OnRemove()
        {
            base.OnRemove();
            //OnUnregisterChangeAttributeCopHandler();
        }
        /**
                private void OnRegisterChangeAttributeCopHandler()
                {
                    ConnectionFeture connection = Entity.GetFeture<ConnectionFeture>();
                    if (connection != null)
                    {
                        connection.RegisterHandler(NetMsgId.Msg_Create_Player2, Protocol_100);
                        connection.RegisterHandler(NetMsgId.Msg_Create_Player2, Protocol_100);
                    }
                }
                private void OnUnregisterChangeAttributeCopHandler()
                {
                    ConnectionFeture connection = Entity.GetFeture<ConnectionFeture>();
                    if (connection != null)
                    {
                        connection.UnregisterHandler(NetMsgId.Msg_Create_Player2);
                        connection.UnregisterHandler(NetMsgId.Msg_Create_Player2);
                    }
                }
        **/

        [Recv(NetMsgId.Msg_Create_Player2)]
        private void RecvTest(string name)
        {
        }



    }
}
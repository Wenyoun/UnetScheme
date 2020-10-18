using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }

        public void Register()
        {
            Connection.RegisterHandler(NetMsgId.Sync_Attribute, OnSyncAttribute);

            SendLoginReq();
        }

        public void Unregister()
        {
            Connection.UnregisterHandler(NetMsgId.Sync_Attribute);
        }

        private void OnSyncAttribute(ChannelMessage msg)
        {
            ByteBuffer buffer = msg.Buffer;
            uint eid = buffer.ReadUInt();
            uint syncId = buffer.ReadUInt();
            Entity entity = Client.Ins.EntityMgr.GetEntity(eid);
            if (entity != null)
            {
                ISyncAttribute attribute = entity.GetSyncAttribute<ISyncAttribute>(syncId);
                if (attribute != null)
                {
                    attribute.Deserialize(buffer);
                    entity.Dispatcher(MessageConstants.Sync_Attribute);
                }
            }
        }

        private void SendLoginReq()
        {
            int[] v1 = new int[] { 10,20,30};
            LoginData v2 = new LoginData();
            v2.Username = "username";
            v2.Password = "password";
            
            ClientSender.RpcLogin(1,
                true,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                "10",
                Vector2.one,
                Vector3.one,
                Vector4.one,
                Quaternion.identity,
                v1,
                v2);
        }
    }
}
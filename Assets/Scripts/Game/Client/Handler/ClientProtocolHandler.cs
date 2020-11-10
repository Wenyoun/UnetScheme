using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Proto;

namespace Zyq.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        public World World { get; set; }
        public Connection Connection { get; set; }

        public void Register()
        {
            Connection.RegisterHandler(MsgId.Sync_Attribute, OnSyncAttribute);
            SendLoginReq();
        }

        public void Unregister()
        {
            Connection.UnregisterHandler(MsgId.Sync_Attribute);
        }

        private void OnSyncAttribute(ChannelMessage msg)
        {
            ByteBuffer buffer = msg.Buffer;
            uint eid = buffer.ReadUInt();
            uint syncId = buffer.ReadUInt();
            Entity entity = World.GetEntity(eid);
            if (entity != null)
            {
                ISyncAttribute attribute = entity.GetSyncAttribute(syncId);
                if (attribute != null)
                {
                    attribute.Deserialize(buffer);
                    entity.Dispatcher(MessageConstants.Sync_Attribute);
                }
            }
        }

        private void SendLoginReq()
        {
            for (int i = 0; i < 1; ++i)
            {
                int[] v1 = new int[1];
                for (int j = 0; j < v1.Length; ++j)
                {
                    v1[j] = j + 1;
                }

                LoginData v2 = new LoginData();

                for (int j = 0; j < 1; ++j)
                {
                    v2.Username += "username" + i;
                }

                for (int j = 0; j < 1; ++j)
                {
                    v2.Password += "password" + i;
                }

                v2.Scores = new int[1];
                for (int j = 0; j < v2.Scores.Length; ++j)
                {
                    v2.Scores[j] = j + 1;
                }

                v2.Logins = new Login[1];
                for (int j = 0; j < v2.Logins.Length; ++j)
                {
                    v2.Logins[j] = (Login) (j % 3);
                }

                v2.Final = i;

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
}
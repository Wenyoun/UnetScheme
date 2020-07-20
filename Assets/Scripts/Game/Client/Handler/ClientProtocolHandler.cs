using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }

        public void Register()
        {
            Connection.RegisterHandler(NetMsgId.Sync_Attribute, OnSyncAttribute);
        }

        public void Unregister()
        {
            Connection.UnregisterHandler(NetMsgId.Sync_Attribute);
        }

        private void OnSyncAttribute(NetworkMessage msg)
        {
            NetworkReader reader = msg.reader;
            uint eid = reader.ReadUInt32();
            uint syncId = reader.ReadUInt32();
            Entity entity = Client.Ins.EntityMgr.GetEntity(eid);
            if (entity != null)
            {
                ISyncAttribute attribute = entity.GetSyncAttribute<ISyncAttribute>(syncId);
                if (attribute != null)
                {
                    attribute.Deserialize(reader);
                    entity.Dispatcher(MessageConstants.Sync_Attribute);
                }
            }
        }

        private void OnTest(NetworkMessage msg)
        {
            NetworkReader reader = msg.reader;
            int v1 = reader.ReadInt32();
            int len1 = reader.ReadInt32();
            int[] v2 = new int[len1];
            for (int i = 0; i < len1; ++i)
            {
                v2[i] = reader.ReadInt32();
            }
            int len2 = reader.ReadInt32();
            LoginData[] v3 = new LoginData[len2];
            for (int i = 0; i < len2; ++i)
            {
                v3[i].testRead(reader);
            }
            //ClientRecver.OnRecvArray(v1, v2, v3);
        }
    }
}
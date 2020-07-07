using Zyq.Game.Base;
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
            byte v1 = reader.ReadByte();
            double v2 = reader.ReadDouble();
            string v3 = reader.ReadString();
            Test1 v4 = new Test1();
            v4.mDeserialize(reader);
            int v5 = reader.ReadInt32();
            Recver.RecvRsp(v1, v2, v3, v4, v5);
        }
    }
}
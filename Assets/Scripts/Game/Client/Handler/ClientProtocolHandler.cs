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
    }
}
using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        public World World { get; set; }
        public Connection Connection { get; set; }

        public void Register()
        {
            Connection.RegisterHandler(MsgID.Sync_Attribute, OnSyncAttribute);
        }

        public void UnRegister()
        {
            Connection.UnRegisterHandler(MsgID.Sync_Attribute);
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
                }
            }
        }
    }
}
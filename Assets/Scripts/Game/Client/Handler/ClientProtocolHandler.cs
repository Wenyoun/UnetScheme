using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class ClientProtocolHandler : AbsProtocolHandler
    {
        private World m_World;

        public void SetWorld(World world)
        {
            m_World = world;
        }


        public override void Register()
        {
            m_Connection.RegisterHandler(MsgID.Sync_Attribute, OnSyncAttribute);
        }

        public override void UnRegister()
        {
            m_Connection.UnRegisterHandler(MsgID.Sync_Attribute);
        }

        private void OnSyncAttribute(ChannelMessage msg)
        {
            ByteBuffer buffer = msg.Buffer;
            uint eid = buffer.ReadUInt();
            uint syncId = buffer.ReadUInt();
            Entity entity = m_World.GetEntity(eid);
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
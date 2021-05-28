using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        private World m_World;

        public void SetWorld(World world)
        {
            m_World = world;
        }


        public void Register()
        {
            Connection.RegisterHandler(MsgID.Sync_Attribute, OnSyncAttribute);
        }

        public void UnRegister()
        {
            Connection.UnRegisterHandler(MsgID.Sync_Attribute);
        }

        public IConnection Connection { get; set; }

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
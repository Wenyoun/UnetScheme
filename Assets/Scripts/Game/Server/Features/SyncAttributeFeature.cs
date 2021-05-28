using Nice.Game.Base;
using System.Collections.Generic;

namespace Nice.Game.Server
{
    public class SyncAttributeFeature : AbsWorldFeature
    {
        private List<Entity> m_List;

        protected override void Init()
        {
            m_List = new List<Entity>();
            m_World.RegisterUpdate(OnUpdate);
        }

        private void OnUpdate(float delta)
        {
            m_List.Clear();
            if (m_World.CopyEntities(m_List))
            {
                int length = m_List.Count;
                for (int i = 0; i < length; ++i)
                {
                    Entity entity = m_List[i];
                    List<ISyncAttribute> attributes = entity.Sync.Attributes;
                    if (attributes.Count > 0)
                    {
                        for (int j = 0; j < attributes.Count; ++j)
                        {
                            ISyncAttribute attribute = attributes[j];
                            if (attribute.IsDirty())
                            {
                                ByteBuffer buffer = ByteBuffer.Allocate(512);
                                buffer.Write(entity.EntityId);
                                buffer.Write(attribute.GetSyncId());
                                attribute.Serialize(buffer);
                                //NetworkServerManager.Broadcast(MsgID.Sync_Attribute, buffer, ChannelType.Reliable);
                            }
                        }
                    }
                }
            }
        }
    }
}
using Nice.Game.Base;
using System.Collections.Generic;

namespace Nice.Game.Server
{
    public static class SyncAttributeMgr
    {
        public static void OnUpdate(World world, List<Entity> entities, float delta)
        {
            if (entities != null)
            {
                int length = entities.Count;
                for (int i = 0; i < length; ++i)
                {
                    Entity entity = entities[i];
                    List<ISyncAttribute> attributes = entity.Sync.Attributes;
                    if (attributes.Count > 0)
                    {
                        for (int j = 0; j < attributes.Count; ++j)
                        {
                            ISyncAttribute attribute = attributes[j];
                            if (attribute.IsDirty())
                            {
                                ByteBuffer buffer = ByteBuffer.Allocate(1400);
                                buffer.Write(entity.EntityId);
                                buffer.Write(attribute.GetSyncId());
                                attribute.Serialize(buffer);
                                world.Broadcast(MsgID.Sync_Attribute, buffer);
                            }
                        }
                    }
                }
            }
        }
    }
}
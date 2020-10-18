using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class SyncAttributeMgr : IUpdate
    {
        public void OnUpdate(float delta)
        {
            List<Entity> entitys = Server.Ins.EntityMgr.Entitys;
            if (entitys != null)
            {
                for (int i = 0; i < entitys.Count; ++i)
                {
                    Entity entity = entitys[i];
                    List<ISyncAttribute> attributes = entity.SyncAttributes.Attributes;
                    if (attributes.Count > 0)
                    {
                        for (int j = 0; j < attributes.Count; ++j)
                        {
                            ISyncAttribute attribute = attributes[j];
                            if (attribute.IsSerialize())
                            {
                                ByteBuffer buffer = ByteBuffer.Allocate(1400);
                                buffer.Write(entity.Eid);
                                buffer.Write(attribute.SyncId);
                                attribute.Serialize(buffer);
                                Server.Ins.Broadcast(NetMsgId.Sync_Attribute, buffer);
                            }
                        }
                    }
                }
            }
        }
    }
}
using System;
using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class SyncAttributeMgr : IUpdate, IDisposable
    {
        private World m_World;

        public SyncAttributeMgr(World world)
        {
            m_World = world;
        }

        public void Dispose()
        {
            m_World = null;
        }

        public void OnUpdate(float delta)
        {
            List<Entity> entitys = m_World.Entitys;
            if (entitys != null)
            {
                for (int i = 0; i < entitys.Count; ++i)
                {
                    Entity entity = entitys[i];
                    List<ISyncAttribute> attributes = entity.Sync.Attributes;
                    if (attributes.Count > 0)
                    {
                        for (int j = 0; j < attributes.Count; ++j)
                        {
                            ISyncAttribute attribute = attributes[j];
                            if (attribute.IsSerialize())
                            {
                                ByteBuffer buffer = ByteBuffer.Allocate(1400);
                                buffer.Write(entity.EntityId);
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
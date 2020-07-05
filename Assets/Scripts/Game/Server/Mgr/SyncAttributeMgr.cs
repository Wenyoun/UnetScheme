﻿using Zyq.Game.Base;
using System.Collections.Generic;
using UnityEngine.Networking;

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
                                NetworkWriter writer = new NetworkWriter();
                                writer.StartMessage(NetMsgId.Sync_Attribute);
                                writer.Write(entity.Eid);
                                writer.Write(attribute.SyncId);
                                attribute.Serialize(writer);
                                writer.FinishMessage();
                                Server.Ins.Broadcast(writer);
                            }
                        }
                    }
                }
            }
        }
    }
}
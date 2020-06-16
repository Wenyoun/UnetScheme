using System;
using Zyq.Game.Base;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class SyncAttributeMgr : IUpdate
    {
        public void OnUpdate(float delta)
        {
            List<Entity> entitys = Server.Ins.EntityMgr.ALL;
            if (entitys != null)
            {
                for (int i = 0; i < entitys.Count; ++i)
                {
                    Entity entity = entitys[i];
                    List<IAttribute> attributes = entity.Attributes.ALL;
                    for (int j = 0; j < attributes.Count; ++j)
                    {
                        IAttribute attribute = attributes[j];
                        if (attribute is ISyncAttribute)
                        {
                            ISyncAttribute sync = (ISyncAttribute)attribute;
                            if (sync.IsSerialize())
                            {
                                NetworkWriter writer = new NetworkWriter();
                                writer.StartMessage(MsgId.Msg_Sync_Field);
                                writer.Write(entity.Eid);
                                writer.Write(sync.SyncId);
                                sync.Serialize(writer);
                                writer.FinishMessage();
                                Server.Ins.Broadcast(null, writer);
                            }
                        }
                    }
                }
            }
        }
    }
}
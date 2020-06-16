using UnityEngine;
using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }

        public void Register()
        {
            Connection.RegisterHandler(MsgId.Msg_Sync_Field, (NetworkMessage msg) =>
            {
                NetworkReader reader = msg.reader;
                uint eid = reader.ReadUInt32();
                uint syncId = reader.ReadUInt32();
                Entity entity = Client.Ins.EntityMgr.GetEntity(eid);
                BaseAttribute attribute = entity.GetAttribute<BaseAttribute>();
                attribute.Deserialize(reader);
                Debug.Log(attribute.Hp1);
            });
        }

        public void Unregister()
        {
            Connection.UnregisterHandler(MsgId.Msg_Sync_Field);
        }
    }
}
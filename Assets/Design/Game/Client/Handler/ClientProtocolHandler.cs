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
                Entity entity = EntityMgr.GetEntity(eid);
            });
        }

        public void Unregister()
        {
            Connection.UnregisterHandler(MsgId.Msg_Sync_Field);
        }
    }
}
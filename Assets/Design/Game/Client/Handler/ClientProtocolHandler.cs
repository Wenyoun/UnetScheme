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
                long dirty = reader.ReadInt64();
                float value = reader.ReadSingle();
                string v = reader.ReadString();

                Debug.Log(eid + ":" + dirty + ":" + value + ":" + v);
            });
        }

        public void Unregister()
        {
            Connection.UnregisterHandler(MsgId.Msg_Sync_Field);
        }
    }
}
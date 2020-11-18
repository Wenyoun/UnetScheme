using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    [SyncClass]
    public class BaseAttribute : ISyncAttribute
    {
        [SyncField] public float Hp1;

        public uint SyncId => 1;

        public bool IsSerialize()
        {
            return false;
        }

        public void Serialize(ByteBuffer writer)
        {
        }

        public void Deserialize(ByteBuffer reader)
        {
        }
    }
}
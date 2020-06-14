using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    [SyncClass]
    public class BaseAttribute : IAttribute, ISync, ISyncStatus
    {
        [SyncField]
        public byte Hp1;
        [SyncField]
        public bool Hp2;
        [SyncField]
        public short Hp3;
        [SyncField]
        public int Hp4;
        [SyncField]
        public long Hp5;
        [SyncField]
        public ushort Hp6;
        [SyncField]
        public uint Hp7;
        [SyncField]
        public ulong Hp8;
        [SyncField]
        public float Hp9;
        [SyncField]
        public double Hp10;
        [SyncField]
        public string Hp11;

        public void Serialize(NetworkWriter writer)
        {
        }

        public void Deserialize(NetworkReader reader)
        {
        }

        public void SyncFinished()
        {
        }
    }
}
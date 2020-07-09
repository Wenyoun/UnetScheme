﻿using UnityEngine;
using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    [SyncClass]
    public class BaseAttribute : ISyncAttribute
    {
        public uint SyncId => 1;

        [SyncField]
        public float Hp1;
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
        [SyncField]
        public Vector3 Hp12;

        public bool IsSerialize()
        {
            return false;
        }

        public void Serialize(NetworkWriter writer)
        {
        }

        public void Deserialize(NetworkReader reader)
        {
        }
    }
}
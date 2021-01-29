using Nice.Game.Base;
using UnityEngine;

namespace Nice.Game.Server
{
    public class World : AbsWorld
    {
        public World() : base(1)
        {
        }

        protected override void Init()
        {
        }

        protected override void Clear()
        {
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            SyncAttributeMgr.OnUpdate(this, Entities.Entitys, delta);
        }
    }
}
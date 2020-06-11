using UnityEngine;

namespace Zyq.Game.Base {
    public sealed class EntityFactory {
        public static Entity CreateTank(uint eid, uint gid, float hp, GameObject target, bool isLocalPlayer, bool isClient) {
            Entity entity = new Entity(eid, gid);
            entity.OnInit();

            entity.AddAttribute(new BaseAttribute(hp));
            entity.AddFeture(new ObjectFeture(target));

            return entity;
        }

        public static Entity CreateShell(uint eid, uint gid, Vector3 velocity, GameObject target, bool isLocalPlayer) {
            Entity entity = new Entity(eid, gid);
            entity.OnInit();
            entity.AddFeture(new ObjectFeture(target));
            return entity;
        }
    }
}
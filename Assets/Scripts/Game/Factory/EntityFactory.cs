using UnityEngine;

namespace Game
{
    public sealed class EntityFactory
    {
        public static Entity CreateTank(uint eid, uint gid, float hp, GameObject target, bool isLocalPlayer, bool isClient)
        {
            Entity entity = new Entity(eid, gid);
            entity.OnInit();

            entity.AddAttribute(new BaseAttribute(hp));
            entity.AddFeture(new ObjectFeture(target));

            if (isLocalPlayer)
            {
                entity.AddCop<TankLocalMovingCop>();
                entity.AddCop<TankLocalShootingCop>();
                target.transform.Find("Camera").gameObject.SetActive(true);
            }
            else
            {
                entity.AddCop<TankSyncMovingCop>();
            }

            if (isClient)
            {
                entity.AddCop<TankUICop>();
            }

            return entity;
        }

        public static Entity CreateShell(uint eid, uint gid, Vector3 velocity, GameObject target, bool isLocalPlayer)
        {
            Entity entity = new Entity(eid, gid);
            entity.OnInit();
            entity.AddFeture(new ObjectFeture(target));
            entity.AddCop(new ShellMovingCop(velocity));
            return entity;
        }
    }
}
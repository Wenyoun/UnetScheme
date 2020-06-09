using UnityEngine;
using UnityEngine.Networking;

namespace Game {
    public class ShellCollision : NetworkBehaviour {
        //[Server]
        private void OnTriggerEnter(Collider collider) {
            /**
            if (collider.gameObject.layer == Layer.Player)
            {
                TankObject tank = collider.gameObject.GetComponent<TankObject>();
                Entity entity = EntityMgr.GetEntity(tank.netId.Value);
                if (entity != null)
                {
                    BaseAttribute attribute = entity.GetAttribute<BaseAttribute>();
                    attribute.Hurt(Constants.Attack);

                    tank.Hp = attribute.CurHp;

                    if (attribute.CurHp <= 0)
                    {
                        tank.RpcDied();
                        GameObject.Destroy(collider.gameObject, 0.1f);
                    }
                }
            }

            //gameObject.GetComponent<ShellObject>().RpcExplosion();

            gameObject.GetComponent<Rigidbody>().detectCollisions = true;

            GameObject.Destroy(gameObject, 0.1f);
            **/
        }
    }
}
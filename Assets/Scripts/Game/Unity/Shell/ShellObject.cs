using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class ShellObject : NetworkBehaviour
    {
        private void Start()
        {
            Debug.Log("ShellObject: " + connectionToServer + "->" + connectionToClient);
        }

        private void Update() 
        {
            Debug.Log("ShellObject: " + connectionToServer + "->" + connectionToClient);
        }

        /**
        [SyncVar]
        public Vector3 Velocity;

        private void Start()
        {
            EntityMgr.AddEntity(EntityFactory.CreateShell(netId.Value, Group.Shell, Velocity, gameObject, isLocalPlayer));
        }

        private void OnDestroy()
        {
            EntityMgr.RemoveEntity(netId.Value);
        }

        [ClientRpc]
        public void RpcExplosion()
        {
            FxMgr.Shot("Effects/ShellExplosion1", transform.position);
        }
        **/
    }
}
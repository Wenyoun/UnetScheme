using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class TankShooting : NetworkBehaviour
    {
        /**
        [SerializeField]
        private Transform m_ShootingSpawn;
        [SerializeField]
        private GameObject m_BulletPrefab;
        **/

        //[Command]
        public void CmdShooting(float force)
        {
            /**
            force = CheckForce(force);
            GameObject shell = Instantiate(m_BulletPrefab, m_ShootingSpawn.position, m_ShootingSpawn.rotation);
            ShellObject shellObject = shell.GetComponent<ShellObject>();
            shellObject.Velocity = m_ShootingSpawn.forward.normalized * force; ;
            GameObject.Destroy(shell, 5);
            NetworkServer.Spawn(shell);
            **/
        }


        //[Command]
        public void CmdForce(float force)
        {
            force = CheckForce(force);
            RpcForce(force);
        }

        //[ClientRpc]
        private void RpcForce(float force)
        {
            EntityMgr.Dispatcher(MessageConstants.Update_Force, netId.Value, FloatBody.Default.Init(force));
        }

        private float CheckForce(float force)
        {
            if (force < Constants.MinForce)
            {
                force = Constants.MinForce;
            }

            if (force > Constants.MaxForce)
            {
                force = Constants.MaxForce;
            }

            return force;
        }
    }
}
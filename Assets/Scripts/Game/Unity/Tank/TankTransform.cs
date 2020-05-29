using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class TankTransform : NetworkBehaviour
    {
        [SyncVar(hook = "OnChangePosition")]
        private Vector3 m_Position;

        [SyncVar(hook = "OnChangeRotation")]
        private Quaternion m_Rotation;

        [Client]
        public void SyncTransform(Vector3 position, Quaternion rotation)
        {
            CmdSyncTransform(position, rotation);
        }

        [Command]
        public void CmdSyncTransform(Vector3 position, Quaternion rotation)
        {
            m_Position = position;
            m_Rotation = rotation;
            EntityMgr.Dispatcher(MessageConstants.Sync_Transform, netId.Value, TransformBody.Default.Init(position, rotation));
        }

        [Client]
        private void OnChangePosition(Vector3 position)
        {
            EntityMgr.Dispatcher(MessageConstants.Sync_Position, netId.Value, Vector3Body.Default.Init(position));
        }

        [Client]
        private void OnChangeRotation(Quaternion rotation)
        {
            EntityMgr.Dispatcher(MessageConstants.Sync_Rotation, netId.Value, QuaternionBody.Default.Init(rotation));
        }
    }
}
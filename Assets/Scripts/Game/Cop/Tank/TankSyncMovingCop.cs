using UnityEngine;

namespace Game
{
    public class TankSyncMovingCop : AbsCop
    {
        private Vector3 m_Position;
        private Quaternion m_Rotation;
        private ObjectFeture m_Object;

        public override void OnInit()
        {
            m_Object = Entity.GetFeture<ObjectFeture>();

            m_Position = m_Object.position;
            m_Rotation = m_Object.rotation;

            RegisterMessage(MessageConstants.Sync_Position, (IBody body) =>
            {
                m_Position = (body as Vector3Body).Value;
            });

            RegisterMessage(MessageConstants.Sync_Rotation, (IBody body) =>
            {
                m_Rotation = (body as QuaternionBody).Value;
            });

            RegisterMessage(MessageConstants.Sync_Transform, (IBody body) =>
            {
                TransformBody b = body as TransformBody;
                m_Position = b.Position;
                m_Rotation = b.Rotation;
            });

            RegisterFixedUpdate(OnFixedUpdate);
        }

        private void OnFixedUpdate(float delta)
        {
            m_Object.position = Vector3.Lerp(m_Object.position, m_Position, Time.deltaTime * 10);
            m_Object.rotation = Quaternion.Slerp(m_Object.rotation, m_Rotation, Time.deltaTime * 10);
        }
    }
}
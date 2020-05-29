using UnityEngine;

namespace Game
{
    public class TankLocalMovingCop : AbsCop
    {
        private float m_ZDeltaPos;
        private float m_YDeltaAngle;

        private Vector3 m_LastPosition;
        private Vector3 m_LastEuler;

        private ObjectFeture m_Object;
        private TankTransform m_Transform;

        public override void OnInit()
        {
            m_Object = Entity.GetFeture<ObjectFeture>();
            m_Transform = m_Object.GetComponent<TankTransform>();

            RegisterUpdate(OnUpdate);
            RegisterFixedUpdate(OnFixedUpdate);

            m_LastPosition = m_Object.transform.position;
            m_LastEuler = m_Object.transform.rotation.eulerAngles;
        }

        private void OnUpdate(float delta)
        {
            m_ZDeltaPos = Input.GetAxis("Vertical") * delta * Constants.Speed;
            m_YDeltaAngle = Input.GetAxis("Horizontal") * delta * Constants.Rotate;

            Vector3 position = m_Object.transform.position;
            Vector3 euler = m_Object.transform.eulerAngles;

            if (Vector3.Distance(position, m_LastPosition) > Constants.Zero || Mathf.Abs(euler.y - m_LastEuler.y) > Constants.Zero)
            {
                m_LastPosition = position;
                m_LastEuler = euler;
                m_Transform.SyncTransform(position, m_Object.transform.rotation);
            }
        }

        private void OnFixedUpdate(float delta)
        {
            m_Object.transform.Rotate(0, m_YDeltaAngle, 0);
            m_Object.transform.Translate(0, 0, m_ZDeltaPos);
        }
    }
}
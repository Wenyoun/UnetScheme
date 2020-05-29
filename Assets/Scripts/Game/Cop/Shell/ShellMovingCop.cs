using UnityEngine;

namespace Game
{
    public class ShellMovingCop : AbsCop
    {
        private Vector3 m_Velocity;
        private ObjectFeture m_Object;

        public ShellMovingCop(Vector3 velocity)
        {
            m_Velocity = velocity;
        }

        public override void OnInit()
        {
            m_Object = Entity.GetFeture<ObjectFeture>();

            RegisterFixedUpdate(OnFixedUpdate);
        }

        private void OnFixedUpdate(float delta)
        {
            m_Object.transform.position += m_Velocity * delta;
        }
    }
}
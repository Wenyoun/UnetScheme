using UnityEngine;

namespace Zyq.Game.Base
{
    public class ObjectFeture : AbsFeture
    {
        private GameObject m_Target;
        private Transform m_Transform;
        private Rigidbody m_Rigidbody;

        public ObjectFeture(GameObject target)
        {
            m_Target = target;
            m_Transform = target.transform;
        }

        public override void OnInit(IEntity entity)
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        public Vector3 position
        {
            get { return m_Transform.position; }
            set { m_Transform.position = value; }
        }

        public Quaternion rotation
        {
            get { return m_Transform.rotation; }
            set { m_Transform.rotation = value; }
        }

        public T GetComponent<T>() where T : Component
        {
            return m_Target.GetComponent<T>();
        }

        public T AddComponent<T>() where T : Component
        {
            T t = m_Target.GetComponent<T>();
            if (t == null)
            {
                t = m_Target.AddComponent<T>();
            }
            return t;
        }

        public GameObject gameObject { get { return m_Target; } }

        public Transform transform { get { return m_Transform; } }
    }
}
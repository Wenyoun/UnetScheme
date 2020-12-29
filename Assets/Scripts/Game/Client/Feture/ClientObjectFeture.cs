using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class ClientObjectFeture : ObjectFeture
    {
        private Transform m_Transform;

        public ClientObjectFeture(string path, Vector3 position)
        {
            m_Transform = ResMgr.CreateRoot(path).transform;
            m_Transform.position = position;
        }

        public override Vector3 scale
        {
            get
            {
                return m_Transform.localScale;
            }
            set
            {
                m_Transform.localScale = value;
            }
        }

        public override Vector3 position
        {
            get
            {
                return m_Transform.localScale;
            }
            set
            {
                m_Transform.localScale = value;
            }
        }

        public override Quaternion rotation
        {
            get
            {
                return m_Transform.rotation;
            }
            set
            {
                m_Transform.rotation = value;
            }
        }
    }
}
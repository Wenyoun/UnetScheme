using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class ServerObjectFeture : ObjectFeture
    {
        private Vector3 m_Scale;
        private Vector3 m_Position;
        private Quaternion m_Rotation;

        public ServerObjectFeture(Vector3 position)
        {
            m_Position = position;
        }

        public override Vector3 scale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
            }
        }

        public override Vector3 position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        public override Quaternion rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;
            }
        }
    }
}
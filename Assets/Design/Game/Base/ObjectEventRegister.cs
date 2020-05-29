using UnityEngine.Networking;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class ObjectEventRegister
    {
        public delegate void MessageDelegate(NetworkBehaviour behaviour);

        public static int AddObject = 1;

        public static int RemoveObject = 2;

        public static ObjectEventRegister Ins = new ObjectEventRegister();

        private Dictionary<int, MessageDelegate> m_Messages;

        private ObjectEventRegister()
        {
            m_Messages = new Dictionary<int, MessageDelegate>();
        }

        public void Dispathcer(int id, NetworkBehaviour behaviour)
        {
            MessageDelegate handler = null;
            if (m_Messages.TryGetValue(id, out handler))
            {
                handler(behaviour);
            }
        }

        public void Register(int id, MessageDelegate handler)
        {
            if (!m_Messages.ContainsKey(id))
            {
                m_Messages.Add(id, handler);
            }
        }

        public void Unregister(int id)
        {
            if (m_Messages.ContainsKey(id))
            {
                m_Messages.Remove(id);
            }
        }
    }
}
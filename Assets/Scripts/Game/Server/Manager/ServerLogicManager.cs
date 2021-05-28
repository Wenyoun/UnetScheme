using System;
using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class ServerLogicManager : IDisposable
    {
        private HDictionary<Type, IServerLogic> m_Logics;

        public ServerLogicManager()
        {
            m_Logics = new HDictionary<Type, IServerLogic>();
        }

        public void Dispose()
        {
            foreach (IServerLogic logic in m_Logics)
            {
                logic.OnRemove();
            }
            m_Logics.Clear();
        }

        public void AddServerLogic<T>(Server server) where T : IServerLogic, new()
        {
            Type t = typeof(T);
            if (!m_Logics.ContainsKey(t))
            {
                T logic = new T();
                logic.OnInit(server);
                m_Logics.Add(t, logic);
            }
        }

        public void RemoveServerLogic<T>() where T : IServerLogic
        {
            Type t = typeof(T);
            if (m_Logics.TryGetValue(t, out IServerLogic logic))
            {
                m_Logics.Remove(t);
                logic.OnRemove();
            }
        }

        public T GetServerLogic<T>() where T : IServerLogic
        {
            Type t = typeof(T);
            if (m_Logics.TryGetValue(t, out IServerLogic logic))
            {
                return (T) logic;
            }
            return default;
        }
    }
}
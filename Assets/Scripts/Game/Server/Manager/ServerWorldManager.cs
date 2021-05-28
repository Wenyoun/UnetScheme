using System;
using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class ServerWorldManager : IDisposable
    {
        private HDictionary<int, AbsWorld> m_Worlds;

        public ServerWorldManager()
        {
            m_Worlds = new HDictionary<int, AbsWorld>();
        }

        public void Dispose()
        {
            foreach (AbsWorld world in m_Worlds)
            {
                world.Dispose();
            }
            m_Worlds.Clear();
        }

        public AbsWorld AddWorld(AbsWorld world)
        {
            if (!m_Worlds.ContainsKey(world.Wid))
            {
                m_Worlds.Add(world.Wid, world);
                world.OnInit();
                return world;
            }
            return null;
        }

        public AbsWorld RemoveWorld(int wid)
        {
            if (m_Worlds.TryGetValue(wid, out AbsWorld world))
            {
                m_Worlds.Remove(wid);
                world.Dispose();
                return world;
            }
            return null;
        }

        public void OnUpdate(float delta)
        {
            foreach (AbsWorld world in m_Worlds)
            {
                world.OnUpdate(delta);
            }
        }

        public void OnFixedUpdate(float delta)
        {
            foreach (AbsWorld world in m_Worlds)
            {
                world.OnFixedUpdate(delta);
            }
        }

        public void OnLateUpdate(float delta)
        {
            foreach (AbsWorld world in m_Worlds)
            {
                world.OnLateUpdate(delta);
            }
        }
    }
}
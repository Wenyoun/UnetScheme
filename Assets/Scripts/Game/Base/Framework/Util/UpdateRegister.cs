using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class UpdateRegister : IUpdate, ILateUpdate, IFixedUpdate, IDisposable
    {
        private List<UpdateDelegate> m_Updates;
        private List<UpdateDelegate> m_TempUpdates;

        private List<LateUpdateDelegate> m_LateUpdates;
        private List<LateUpdateDelegate> m_LateTempUpdates;

        private List<UpdateDelegate> m_FixedUpdates;
        private List<UpdateDelegate> m_TempFixedUpdates;

        public UpdateRegister()
        {
            m_Updates = new List<UpdateDelegate>();
            m_TempUpdates = new List<UpdateDelegate>();

            m_LateUpdates = new List<LateUpdateDelegate>();
            m_LateTempUpdates = new List<LateUpdateDelegate>();

            m_FixedUpdates = new List<UpdateDelegate>();
            m_TempFixedUpdates = new List<UpdateDelegate>();
        }

        public void OnInit()
        {
            Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            m_Updates.Clear();
            m_TempUpdates.Clear();

            m_FixedUpdates.Clear();
            m_TempFixedUpdates.Clear();
        }

        public void OnUpdate(float delta)
        {
            if (m_Updates.Count > 0)
            {
                m_TempUpdates.Clear();
                m_TempUpdates.AddRange(m_Updates);

                for (int i = 0; i < m_TempUpdates.Count; ++i)
                {
                    m_TempUpdates[i](delta);
                }
            }
        }

        public void OnLateUpdate()
        {
            if (m_LateUpdates.Count > 0)
            {
                m_LateTempUpdates.Clear();
                m_LateTempUpdates.AddRange(m_LateUpdates);

                for (int i = 0; i < m_LateTempUpdates.Count; ++i)
                {
                    m_LateTempUpdates[i]();
                }
            }
        }

        public void OnFixedUpdate(float delta)
        {
            if (m_FixedUpdates.Count > 0)
            {
                m_TempFixedUpdates.Clear();
                m_TempFixedUpdates.AddRange(m_FixedUpdates);

                for (int i = 0; i < m_TempFixedUpdates.Count; ++i)
                {
                    m_TempFixedUpdates[i](delta);
                }
            }
        }

        public void RegisterUpdate(UpdateDelegate update)
        {
            if (!m_Updates.Contains(update))
            {
                m_Updates.Add(update);
            }
        }

        public void UnregisterUpdate(UpdateDelegate update)
        {
            if (m_Updates.Contains(update))
            {
                m_Updates.Remove(update);
            }
        }

        public void RegisterLateUpdate(LateUpdateDelegate update)
        {
            if (!m_LateUpdates.Contains(update))
            {
                m_LateUpdates.Add(update);
            }
        }

        public void UnregisterLateUpdate(LateUpdateDelegate update)
        {
            if (m_LateUpdates.Contains(update))
            {
                m_LateUpdates.Remove(update);
            }
        }

        public void RegisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (!m_FixedUpdates.Contains(fixedUpdate))
            {
                m_FixedUpdates.Add(fixedUpdate);
            }
        }

        public void UnregisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (m_FixedUpdates.Contains(fixedUpdate))
            {
                m_FixedUpdates.Remove(fixedUpdate);
            }
        }
    }
}
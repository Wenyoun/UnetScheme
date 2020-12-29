using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class UpdaterRegister : IUpdate, IFixedUpdate, IDisposable
    {
        private List<WrapperUpdate> m_Updates;
        private List<WrapperUpdate> m_RemoveUpdates;

        private List<WrapperFixedUpdate> m_FixedUpdates;
        private List<WrapperFixedUpdate> m_RemoveFixedUpdates;

        public UpdaterRegister()
        {
            m_Updates = new List<WrapperUpdate>(64);
            m_RemoveUpdates = new List<WrapperUpdate>(16);

            m_FixedUpdates = new List<WrapperFixedUpdate>(64);
            m_RemoveFixedUpdates = new List<WrapperFixedUpdate>(16);
        }

        public void Dispose()
        {
            m_Updates.Clear();
            m_RemoveUpdates.Clear();

            m_FixedUpdates.Clear();
            m_RemoveFixedUpdates.Clear();
        }

        public void OnUpdate(float delta)
        {
            TickRemoveUpdates();
            TickUpdates(delta);
        }

        public void OnFixedUpdate(float delta)
        {
            TickRemoveFixedUpdates();
            TickFixedUpdate(delta);
        }

        public void Register(UpdateDelegate handler)
        {
            if (SearchUpdateIndex(handler) == -1)
            {
                m_Updates.Add(new WrapperUpdate(handler));
            }
        }

        public void UnRegister(UpdateDelegate handler)
        {
            int index = SearchUpdateIndex(handler);
            if (index >= 0)
            {
                WrapperUpdate w = m_Updates[index];
                w.IsRemove = true;
                m_Updates[index] = w;
                m_RemoveUpdates.Add(w);
            }
        }

        public void Register(FixedUpdateDelegate handler)
        {
            if (SearchFixedUpdateIndex(handler) == -1)
            {
                m_FixedUpdates.Add(new WrapperFixedUpdate(handler));
            }
        }

        public void UnRegister(FixedUpdateDelegate handler)
        {
            int index = SearchFixedUpdateIndex(handler);
            if (index >= 0)
            {
                WrapperFixedUpdate w = m_FixedUpdates[index];
                w.IsRemove = true;
                m_FixedUpdates[index] = w;
                m_RemoveFixedUpdates.Add(w);
            }
        }

        private void TickUpdates(float delta)
        {
            int length = m_Updates.Count;
            for (int i = 0; i < length; ++i)
            {
                m_Updates[i].Invoke(delta);
            }
        }

        private void TickRemoveUpdates()
        {
            int length = m_RemoveUpdates.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    WrapperUpdate w = m_RemoveUpdates[i];
                    int index = SearchUpdateIndex(w.Handler);
                    if (index >= 0)
                    {
                        m_Updates.RemoveAt(index);
                    }
                }
                m_RemoveUpdates.Clear();
            }
        }

        private void TickFixedUpdate(float delta)
        {
            int length = m_FixedUpdates.Count;
            for (int i = 0; i < length; ++i)
            {
                m_FixedUpdates[i].Invoke(delta);
            }
        }

        public void TickRemoveFixedUpdates()
        {
            int length = m_RemoveFixedUpdates.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    WrapperFixedUpdate w = m_RemoveFixedUpdates[i];
                    int index = SearchFixedUpdateIndex(w.Handler);
                    if (index >= 0)
                    {
                        m_FixedUpdates.RemoveAt(index);
                    }
                }
                m_RemoveFixedUpdates.Clear();
            }
        }

        private int SearchUpdateIndex(UpdateDelegate handler)
        {
            int index = -1;
            int length = m_Updates.Count;
            for (int i = 0; i < length; ++i)
            {
                if (m_Updates[i].IsEquals(handler))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private int SearchFixedUpdateIndex(FixedUpdateDelegate handler)
        {
            int index = -1;
            int length = m_FixedUpdates.Count;
            for (int i = 0; i < length; ++i)
            {
                if (m_FixedUpdates[i].IsEquals(handler))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private struct WrapperUpdate
        {
            public bool IsRemove;
            public UpdateDelegate Handler;

            public WrapperUpdate(UpdateDelegate handler)
            {
                IsRemove = false;
                Handler = handler;
            }

            public void Invoke(float delta)
            {
                if (!IsRemove)
                {
                    Handler.Invoke(delta);
                }
            }

            public bool IsEquals(UpdateDelegate handler)
            {
                return Handler == handler;
            }
        }

        private struct WrapperFixedUpdate
        {
            public bool IsRemove;
            public FixedUpdateDelegate Handler;

            public WrapperFixedUpdate(FixedUpdateDelegate handler)
            {
                IsRemove = false;
                Handler = handler;
            }

            public void Invoke(float delta)
            {
                if (!IsRemove)
                {
                    Handler.Invoke(delta);
                }
            }

            public bool IsEquals(FixedUpdateDelegate handler)
            {
                return Handler == handler;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class UpdaterRegister : IDisposable
    {
        private List<Wrapper> m_Updates;
        private List<Wrapper> m_RemoveUpdates;

        public UpdaterRegister()
        {
            m_Updates = new List<Wrapper>(64);
            m_RemoveUpdates = new List<Wrapper>(16);
        }

        public void Dispose()
        {
            m_Updates.Clear();
            m_RemoveUpdates.Clear();
        }

        public void OnUpdate(float delta)
        {
            TickRemoveUpdates();
            TickUpdates(delta);
        }

        public void Register(UpdateDelegate handler)
        {
            if (SearchUpdateIndex(handler) == -1)
            {
                m_Updates.Add(new Wrapper(handler));
            }
        }

        public void UnRegister(UpdateDelegate handler)
        {
            int index = SearchUpdateIndex(handler);
            if (index >= 0)
            {
                Wrapper w = m_Updates[index];
                w.IsRemove = true;
                m_Updates[index] = w;
                m_RemoveUpdates.Add(w);
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
                    Wrapper w = m_RemoveUpdates[i];
                    int index = SearchUpdateIndex(w.Handler);
                    if (index >= 0)
                    {
                        m_Updates.RemoveAt(index);
                    }
                }
                m_RemoveUpdates.Clear();
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

        private struct Wrapper
        {
            public bool IsRemove;
            public UpdateDelegate Handler;

            public Wrapper(UpdateDelegate handler)
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
    }
}
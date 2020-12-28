using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class MessagerRegister : IUpdate, IMessage, IDisposable
    {
        private List<Wrapper> m_Removes;
        private Dictionary<int, List<Wrapper>> m_Wrappers;

        public MessagerRegister()
        {
            m_Removes = new List<Wrapper>();
            m_Wrappers = new Dictionary<int, List<Wrapper>>();
        }

        public void Dispose()
        {
            m_Removes.Clear();
            m_Wrappers.Clear();
        }

        public void OnUpdate(float delta)
        {
            int length = m_Removes.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    Wrapper wp = m_Removes[i];
                    if (m_Wrappers.TryGetValue(wp.Id, out List<Wrapper> wps))
                    {
                        int index = SearchIndex(wps, wp.Handler);
                        if (index >= 0)
                        {
                            wps.RemoveAt(index);
                        }
                    }
                }
                m_Removes.Clear();
            }
        }

        public void Register(int id, MsgDelegate handler)
        {
            List<Wrapper> wps;
            if (!m_Wrappers.TryGetValue(id, out wps))
            {
                wps = new List<Wrapper>();
                m_Wrappers.Add(id, wps);
            }

            int index = SearchIndex(wps, handler);
            if (index == -1)
            {
                wps.Add(new Wrapper(id, handler));
            }
        }

        public void UnRegister(int id, MsgDelegate handler)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int index = SearchIndex(wps, handler);
                if (index >= 0)
                {
                    Wrapper wp = wps[index];
                    wp.IsRemove = true;
                    wps[index] = wp;
                    m_Removes.Add(wp);
                }
            }
        }

        public void Dispatcher(int id, IBody body = null)
        {
            List<Wrapper> wps;
            if (m_Wrappers.TryGetValue(id, out wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(body);
                }
            }
        }

        private int SearchIndex(List<Wrapper> wps, MsgDelegate handler)
        {
            int index = -1;
            int length = wps.Count;
            for (int i = 0; i < length; ++i)
            {
                if (wps[i].IsEquals(handler))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private struct Wrapper
        {
            public int Id;
            public bool IsRemove;
            public MsgDelegate Handler;

            public Wrapper(int id, MsgDelegate handler)
            {
                Id = id;
                IsRemove = false;
                Handler = handler;
            }

            public void Invoke(IBody body)
            {
                if (!IsRemove)
                {
                    Handler.Invoke(body);
                }
            }

            public bool IsEquals(MsgDelegate handler)
            {
                return Handler == handler;
            }
        }
    }
}
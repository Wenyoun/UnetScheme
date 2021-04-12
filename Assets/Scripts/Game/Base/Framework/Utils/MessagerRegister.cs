using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class MessagerRegister : IUpdate, IDisposable
    {
        private List<Wrapper> m_Removes;
        private Dictionary<int, List<Wrapper>> m_Wrappers;

        public MessagerRegister()
        {
            m_Removes = new List<Wrapper>(16);
            m_Wrappers = new Dictionary<int, List<Wrapper>>(32);
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

        public void Dispatcher(int id)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(this);
                }
            }
        }

        public void Dispatcher<T1>(int id, T1 t1)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(this, t1);
                }
            }
        }

        public void Dispatcher<T1, T2>(int id, T1 t1, T2 t2)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(this, t1, t2);
                }
            }
        }

        public void Dispatcher<T1, T2, T3>(int id, T1 t1, T2 t2, T3 t3)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(this, t1, t2, t3);
                }
            }
        }

        public void Dispatcher<T1, T2, T3, T4>(int id, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(this, t1, t2, t3, t4);
                }
            }
        }

        public void Dispatcher<T1, T2, T3, T4, T5>(int id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
            {
                int length = wps.Count;
                for (int i = 0; i < length; ++i)
                {
                    wps[i].Invoke(this, t1, t2, t3, t4, t5);
                }
            }
        }

        public void Register(int id, Action<Body> handler)
        {
            RegisterDelegate(id, handler);
        }

        public void Register<T1>(int id, Action<Body, T1> handler)
        {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2>(int id, Action<Body, T1, T2> handler)
        {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2, T3>(int id, Action<Body, T1, T2, T3> handler)
        {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2, T3, T4>(int id, Action<Body, T1, T2, T3, T4> handler)
        {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2, T3, T4, T5>(int id, Action<Body, T1, T2, T3, T4, T5> handler)
        {
            RegisterDelegate(id, handler);
        }

        public void UnRegister(int id, Delegate handler)
        {
            UnRegisterDelegate(id, handler);
        }

        public void UnRegister(int id, Action<Body> handler)
        {
            UnRegisterDelegate(id, handler);
        }

        public void UnRegister<T1>(int id, Action<Body, T1> handler)
        {
            UnRegisterDelegate(id, handler);
        }

        public void UnRegister<T1, T2>(int id, Action<Body, T1, T2> handler)
        {
            UnRegisterDelegate(id, handler);
        }

        public void UnRegister<T1, T2, T3>(int id, Action<Body, T1, T2, T3> handler)
        {
            UnRegisterDelegate(id, handler);
        }

        public void UnRegister<T1, T2, T3, T4>(int id, Action<Body, T1, T2, T3, T4> handler)
        {
            UnRegisterDelegate(id, handler);
        }

        public void UnRegister<T1, T2, T3, T4, T5>(int id, Action<Body, T1, T2, T3, T4, T5> handler)
        {
            UnRegisterDelegate(id, handler);
        }

        private void RegisterDelegate(int id, Delegate handler)
        {
            if (!m_Wrappers.TryGetValue(id, out List<Wrapper> wps))
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

        private void UnRegisterDelegate(int id, Delegate handler)
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

        private int SearchIndex(List<Wrapper> wps, Delegate handler)
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
            public Delegate Handler;

            public Wrapper(int id, Delegate handler)
            {
                Id = id;
                IsRemove = false;
                Handler = handler;
            }

            public void Invoke(MessagerRegister register)
            {
                if (!IsRemove)
                {
                    ((Action<Body>) Handler).Invoke(new Body(Id, Handler, register));
                }
            }

            public void Invoke<T1>(MessagerRegister register, T1 t1)
            {
                if (!IsRemove)
                {
                    ((Action<Body, T1>) Handler).Invoke(new Body(Id, Handler, register), t1);
                }
            }

            public void Invoke<T1, T2>(MessagerRegister register, T1 t1, T2 t2)
            {
                if (!IsRemove)
                {
                    ((Action<Body, T1, T2>) Handler).Invoke(new Body(Id, Handler, register), t1, t2);
                }
            }

            public void Invoke<T1, T2, T3>(MessagerRegister register, T1 t1, T2 t2, T3 t3)
            {
                if (!IsRemove)
                {
                    ((Action<Body, T1, T2, T3>) Handler).Invoke(new Body(Id, Handler, register), t1, t2, t3);
                }
            }

            public void Invoke<T1, T2, T3, T4>(MessagerRegister register, T1 t1, T2 t2, T3 t3, T4 t4)
            {
                if (!IsRemove)
                {
                    ((Action<Body, T1, T2, T3, T4>) Handler).Invoke(new Body(Id, Handler, register), t1, t2, t3, t4);
                }
            }

            public void Invoke<T1, T2, T3, T4, T5>(MessagerRegister register, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
            {
                if (!IsRemove)
                {
                    ((Action<Body, T1, T2, T3, T4, T5>) Handler).Invoke(new Body(Id, Handler, register), t1, t2, t3, t4, t5);
                }
            }

            public bool IsEquals(Delegate handler)
            {
                return Handler == handler;
            }
        }
    }
}
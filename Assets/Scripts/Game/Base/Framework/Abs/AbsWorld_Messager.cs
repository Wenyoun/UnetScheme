using System;
using UnityEngine.Assertions.Must;

namespace Nice.Game.Base
{
    public partial class AbsWorld
    {
        public void DispatchMessage(int id)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Dispatcher(id);
        }

        public void DispatchMessage<T1>(int id, T1 t1)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Dispatcher(id, t1);
        }

        public void DispatchMessage<T1, T2>(int id, T1 t1, T2 t2)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Dispatcher(id, t1, t2);
        }

        public void DispatchMessage<T1, T2, T3>(int id, T1 t1, T2 t2, T3 t3)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Dispatcher(id, t1, t2, t3);
        }

        public void DispatchMessage<T1, T2, T3, T4>(int id, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Dispatcher(id, t1, t2, t3, t4);
        }

        public void DispatchMessage<T1, T2, T3, T4, T5>(int id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Dispatcher(id, t1, t2, t3, t4, t5);
        }

        public void RegisterMessage(int id, Action<Body> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Register(id, handler);
        }

        public void RegisterMessage<T1>(int id, Action<Body, T1> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Register(id, handler);
        }

        public void RegisterMessage<T1, T2>(int id, Action<Body, T1, T2> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Register(id, handler);
        }

        public void RegisterMessage<T1, T2, T3>(int id, Action<Body, T1, T2, T3> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Register(id, handler);
        }

        public void RegisterMessage<T1, T2, T3, T4>(int id, Action<Body, T1, T2, T3, T4> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Register(id, handler);
        }

        public void RegisterMessage<T1, T2, T3, T4, T5>(int id, Action<Body, T1, T2, T3, T4, T5> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.Register(id, handler);
        }

        public void UnRegisterMessage(int id, Delegate handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }

        public void UnRegisterMessage(int id, Action<Body> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }

        public void UnRegisterMessage<T1>(int id, Action<Body, T1> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }

        public void UnRegisterMessage<T1, T2>(int id, Action<Body, T1, T2> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }

        public void UnRegisterMessage<T1, T2, T3>(int id, Action<Body, T1, T2, T3> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }

        public void UnRegisterMessage<T1, T2, T3, T4>(int id, Action<Body, T1, T2, T3, T4> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }

        public void UnRegisterMessage<T1, T2, T3, T4, T5>(int id, Action<Body, T1, T2, T3, T4, T5> handler)
        {
            if (m_Dispose)
            {
                return;
            }
            m_Messager.UnRegister(id, handler);
        }
    }
}
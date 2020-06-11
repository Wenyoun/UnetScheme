using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class MsgRegister : IMessage, IDisposable
    {
        private List<MsgDelegate> m_Temp;
        private Dictionary<int, List<MsgDelegate>> m_Handlers;

        public MsgRegister()
        {
            m_Temp = new List<MsgDelegate>();
            m_Handlers = new Dictionary<int, List<MsgDelegate>>();

            OnInit();
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
            m_Temp.Clear();
            m_Handlers.Clear();
        }

        public void Register(int id, MsgDelegate handler)
        {
            List<MsgDelegate> evts = null;
            if (!m_Handlers.TryGetValue(id, out evts))
            {
                evts = new List<MsgDelegate>();
                m_Handlers.Add(id, evts);
            }
            if (!evts.Contains(handler))
            {
                evts.Add(handler);
            }
        }

        public void Unregister(int id, MsgDelegate handler)
        {
            List<MsgDelegate> evts = null;
            if (m_Handlers.TryGetValue(id, out evts))
            {
                if (evts.Contains(handler))
                {
                    evts.Remove(handler);
                }
            }
        }

        public void Dispatcher(int id, IBody body = null)
        {
            List<MsgDelegate> evts = null;
            if (m_Handlers.TryGetValue(id, out evts))
            {
                m_Temp.Clear();
                m_Temp.AddRange(evts);
                for (int i = 0; i < m_Temp.Count; ++i)
                {
                    m_Temp[i](body);
                }
            }
        }
    }
}
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
        }

        public void Dispose()
        {
            m_Temp.Clear();
            m_Handlers.Clear();
        }

        public void Register(int msgId, MsgDelegate handler)
        {
            List<MsgDelegate> evts;
            if (!m_Handlers.TryGetValue(msgId, out evts))
            {
                evts = new List<MsgDelegate>();
                m_Handlers.Add(msgId, evts);
            }

            if (!evts.Contains(handler))
            {
                evts.Add(handler);
            }
        }

        public void Unregister(int msgId, MsgDelegate handler)
        {
            List<MsgDelegate> evts;
            if (m_Handlers.TryGetValue(msgId, out evts))
            {
                if (evts.Contains(handler))
                {
                    evts.Remove(handler);
                }
            }
        }

        public void Dispatcher(int msgId, IBody body = null)
        {
            List<MsgDelegate> evts;
            if (m_Handlers.TryGetValue(msgId, out evts))
            {
                m_Temp.Clear();
                m_Temp.AddRange(evts);
                int length = m_Temp.Count;
                if (length > 0)
                {
                    for (int i = 0; i < length; ++i)
                    {
                        m_Temp[i].Invoke(body);
                    }
                }
            }
        }
    }
}
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Connection : IDisposable
    {
        private NetworkConnection m_Net;
        private List<IProtocolHandler> m_Handlers;

        public Connection(NetworkConnection net)
        {
            m_Net = net;
            Net = net;
            m_Handlers = new List<IProtocolHandler>();
        }

        public void RegisterProtocol<T>() where T : IProtocolHandler, new()
        {
            IProtocolHandler handler = new T();
            handler.Register(this);
            m_Handlers.Add(handler);
        }

        public void ClearProtocols()
        {
            for (int i = 0; i < m_Handlers.Count; ++i)
            {
                m_Handlers[i].Unregister(this);
            }
            m_Handlers.Clear();
        }

        public void Dispose()
        {
            ClearProtocols();
            Net = null;
            m_Handlers = null;
        }

        public NetworkConnection Net { get; private set; }
    }
}
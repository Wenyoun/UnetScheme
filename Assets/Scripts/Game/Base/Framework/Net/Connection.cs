using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using Base.Net.Impl;

namespace Zyq.Game.Base
{
    public class Connection : IDisposable
    {
        private NetworkConnection m_Network;
        private List<IProtocolHandler> m_Handlers;

        public Connection()
        {
            m_Handlers = new List<IProtocolHandler>();
        }

        public void OnConnect(NetworkConnection network)
        {
            m_Network = network;
        }

        public void OnDisconnect(NetworkConnection network)
        {
            ClearRegisterProtocols();
        }

        public void RegisterProtocol<T>() where T : IProtocolHandler, new()
        {
            IProtocolHandler handler = new T();
            handler.Connection = this;
            handler.Register();
            m_Handlers.Add(handler);
        }

        public void Dispose()
        {
            ClearRegisterProtocols();
            m_Network.Dispose();
        }

        public void RegisterHandler(short id, NetworkMessageDelegate handler)
        {
            m_Network.RegisterHandler(id, handler);
        }

        public void UnregisterHandler(short id)
        {
            m_Network.UnregisterHandler(id);
        }

        public void Send(NetworkWriter writer)
        {
            m_Network.SendWriter(writer, 0);
        }

        private void ClearRegisterProtocols()
        {
            if (m_Handlers != null)
            {
                for (int i = 0; i < m_Handlers.Count; ++i)
                {
                    m_Handlers[i].Unregister();
                }
                m_Handlers.Clear();
                m_Handlers = null;
            }
        }
    }
}
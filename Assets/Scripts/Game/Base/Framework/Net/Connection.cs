using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public class Connection : IDisposable
    {
        private bool m_IsConnected;
        private NetworkConnection m_Network;
        private List<IProtocolHandler> m_Handlers;

        public Connection()
        {
            m_Handlers = new List<IProtocolHandler>();
        }

        public void OnConnect(NetworkConnection network)
        {
            m_IsConnected = true;
            m_Network = network;
        }

        public void OnDisconnect(NetworkConnection network)
        {
            m_IsConnected = false;
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
            m_IsConnected = false;
            ClearRegisterProtocols();
            m_Network.Dispose();
            m_Network = null;
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
            if (m_IsConnected)
            {
                m_Network.SendWriter(writer, 0);
            }
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
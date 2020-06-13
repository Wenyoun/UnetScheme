using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Zyq.Game.Base {
    public class Connection : IDisposable {
        private NetworkConnection m_Net;
        private List<IProtocolHandler> m_Handlers;

        public Connection(NetworkConnection net) {
            m_Net = net;
            m_Handlers = new List<IProtocolHandler>();
        }

        public void RegisterProtocol<T>() where T : IProtocolHandler, new() {
            IProtocolHandler handler = new T();
            handler.Connection = this;
            handler.Register();
            m_Handlers.Add(handler);
        }

        public void ClearRegisterProtocols() {
            for (int i = 0; i < m_Handlers.Count; ++i) {
                m_Handlers[i].Unregister();
            }
            m_Handlers.Clear();
        }

        public void Dispose() {
            ClearRegisterProtocols();

            m_Net.Dispose();
            m_Handlers.Clear();

            m_Net = null;
            m_Handlers = null;
        }

        public void RegisterHandler(short id, NetworkMessageDelegate handler) {
            m_Net.RegisterHandler(id, handler);
        }

        public void UnregisterHandler(short id) {
            m_Net.UnregisterHandler(id);
        }

        public void Send(short id, MessageBase msg) {
            m_Net.Send(id, msg);
        }

        public void Send(NetworkWriter writer) {
            m_Net.SendWriter(writer, 0);
        }
    }
}
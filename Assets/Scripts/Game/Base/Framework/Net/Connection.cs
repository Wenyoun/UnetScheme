using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Connection : IDisposable
    {
        private IChannel m_Channel;
        private List<IProtocolHandler> m_Handlers;

        public Connection(IChannel channel)
        {
            m_Channel = channel;
            m_Handlers = new List<IProtocolHandler>();
        }

        public void Dispose()
        {
            ClearRegisterProtocols();
            m_Channel.Dispose();
        }

        public T RegisterProtocol<T>() where T : IProtocolHandler, new()
        {
            if (IsNotFind<T>())
            {
                T handler = new T();
                handler.Connection = this;
                handler.Register();
                m_Handlers.Add(handler);
                return handler;
            }
            return default(T);
        }

        public void RegisterHandler(ushort cmd, ChannelMessageDelegate handler)
        {
            m_Channel.Register(cmd, handler);
        }

        public void UnregisterHandler(ushort cmd)
        {
            m_Channel.Unregister(cmd);
        }

        public void Send(ushort cmd, ByteBuffer buffer)
        {
            m_Channel.Send(cmd, buffer);
        }

        public long ConnectionId
        {
            get { return m_Channel.ChannelId; }
        }

        private void ClearRegisterProtocols()
        {
            int length = m_Handlers.Count;
            for (int i = 0; i < length; ++i)
            {
                m_Handlers[i].UnRegister();
            }
            m_Handlers.Clear();
        }

        private bool IsNotFind<T>() where T : IProtocolHandler
        {
            Type t = typeof(T);
            bool isNotFind = true;
            int length = m_Handlers.Count;
            for (int i = 0; i < length; ++i)
            {
                if (t == m_Handlers[i].GetType())
                {
                    isNotFind = false;
                    break;
                }
            }
            return isNotFind;
        }
    }
}
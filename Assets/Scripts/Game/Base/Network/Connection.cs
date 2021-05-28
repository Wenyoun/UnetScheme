using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    internal class Connection : IConnection
    {
        private IChannel m_Channel;
        private List<IProtocolHandler> m_Protocols;

        public Connection(IChannel channel)
        {
            m_Channel = channel;
            m_Protocols = new List<IProtocolHandler>();
        }

        internal virtual void OnUpdate()
        {
            m_Channel.OnUpdate();
        }

        public void Dispose()
        {
            ClearRegisterProtocols();
            m_Channel.Dispose();
        }

        public void Disconnect()
        {
            ClearRegisterProtocols();
            m_Channel.Disconnect();
        }

        public T RegisterProtocol<T>() where T : IProtocolHandler, new()
        {
            if (IsNotFind<T>())
            {
                T handler = new T();
                handler.Connection = this;
                handler.Register();
                m_Protocols.Add(handler);
                return handler;
            }
            return default;
        }

        public void RegisterHandler(ushort cmd, ChannelMessageDelegate handler)
        {
            m_Channel.Register(cmd, handler);
        }

        public void UnRegisterHandler(ushort cmd)
        {
            m_Channel.UnRegister(cmd);
        }

        public void Send(ushort cmd, ByteBuffer buffer, ChannelType channel)
        {
            if (!m_Channel.IsConnected)
            {
                return;
            }

            if (channel == ChannelType.Unreliable)
            {
                const int length = KcpConstants.Packet_Length - 100;
                if (buffer.ReadableLength > length)
                {
                    throw new ArgumentException($"invalid length ChannelType = {channel}, ByteBuffer.ReadableLength > {length}");
                }
            }

            m_Channel.Send(cmd, buffer, channel);
        }

        public uint ConnectionId
        {
            get { return m_Channel.ChannelId; }
        }

        public bool IsConnected
        {
            get { return m_Channel.IsConnected; }
        }

        private void ClearRegisterProtocols()
        {
            int length = m_Protocols.Count;
            for (int i = 0; i < length; ++i)
            {
                m_Protocols[i].UnRegister();
            }
            m_Protocols.Clear();
        }

        private bool IsNotFind<T>() where T : IProtocolHandler
        {
            Type t = typeof(T);
            bool isNotFind = true;
            int length = m_Protocols.Count;
            for (int i = 0; i < length; ++i)
            {
                if (t == m_Protocols[i].GetType())
                {
                    isNotFind = false;
                    break;
                }
            }
            return isNotFind;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Connection : IDisposable
    {
        private IChannel channel;
        private List<IProtocolHandler> handlers;

        public Connection()
        {
            handlers = new List<IProtocolHandler>();
        }

        public void OnConnect(IChannel channel)
        {
            this.channel = channel;
        }

        public void OnDisconnect(IChannel channel)
        {
            ClearRegisterProtocols();
        }

        public void RegisterProtocol<T>() where T : IProtocolHandler, new()
        {
            IProtocolHandler handler = new T();
            handler.Connection = this;
            handler.Register();
            handlers.Add(handler);
        }

        public void Dispose()
        {
            ClearRegisterProtocols();
            channel.Dispose();
        }

        public void RegisterHandler(ushort cmd, ChannelMessageDelegate handler)
        {
            channel.Register(cmd, handler);
        }

        public void UnregisterHandler(ushort cmd)
        {
            channel.Unregister(cmd);
        }

        public void Send(ushort cmd, ByteBuffer buffer)
        {
            channel.Send(cmd, buffer);
        }

        private void ClearRegisterProtocols()
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                handlers[i].Unregister();
            }
            handlers.Clear();
        }
    }
}
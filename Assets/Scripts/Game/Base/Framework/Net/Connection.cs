using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Connection : IDisposable
    {
        private IChannel channel;
        private List<IProtocolHandler> handlers;

        public Connection(IChannel channel)
        {
            this.channel = channel;
            handlers = new List<IProtocolHandler>();
        }

        public void Dispose()
        {
            ClearRegisterProtocols();
            channel.Dispose();
        }

        public void Dispatcher()
        {
            channel.Dispatcher();
        }

        public void RegisterProtocol<T>() where T : IProtocolHandler, new()
        {
            IProtocolHandler handler = new T();
            handler.Connection = this;
            handler.Register();
            handlers.Add(handler);
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

        public long ConnectionId
        {
            get { return channel.ChannelId; }
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
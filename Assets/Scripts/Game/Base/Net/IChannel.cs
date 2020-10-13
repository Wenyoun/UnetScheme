using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public delegate void ChannelMessageDelegate(ChannelMessage channelMessage);

    public struct ChannelMessage
    {
        public int Cmd;
        public ByteBuffer Buffer;
        public IChannel Channel;

        public ChannelMessage(int cmd, ByteBuffer buffer, IChannel channel)
        {
            Cmd = cmd;
            Buffer = buffer;
            Channel = channel;
        }
    }

    public interface IChannel : IDisposable
    {
        long ChannelId { get; }
        
        bool IsConnected { get; }

        void Send(Packet packet);

        void Dispatcher();

        void Disconnect();
        
        void Unregister(ushort cmd);
        
        void Register(ushort cmd, ChannelMessageDelegate handler);
    }

    public abstract class AbstractChannel : IChannel
    {
        protected Dictionary<ushort, ChannelMessageDelegate> handlers;

        public AbstractChannel()
        {
            handlers = new Dictionary<ushort, ChannelMessageDelegate>();
        }

        public void Register(ushort cmd, ChannelMessageDelegate handler)
        {
            if (!handlers.ContainsKey(cmd))
            {
                handlers.Add(cmd, handler);
            }
        }

        public void Unregister(ushort cmd)
        {
            if (handlers.ContainsKey(cmd))
            {
                handlers.Remove(cmd);
            }
        }

        public void ClearHandlers()
        {
            if (handlers != null)
            {
                handlers.Clear();
            }
        }

        public virtual void Dispose()
        {
            handlers = null;
        }

        public abstract void Dispatcher();

        public abstract void Disconnect();

        public abstract long ChannelId { get; }

        public abstract bool IsConnected { get; }

        public abstract void Send(Packet packet);
    }
}
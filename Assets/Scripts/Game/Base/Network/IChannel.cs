using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public delegate void ChannelMessageDelegate(ChannelMessage channelMessage);

    public struct ChannelMessage
    {
        public int Cmd;
        public ByteBuffer Buffer;

        public ChannelMessage(int cmd, ByteBuffer buffer)
        {
            Cmd = cmd;
            Buffer = buffer;
        }
    }

    public interface IChannel : IDisposable
    {
        long ChannelId { get; }

        bool IsConnected { get; }

        void Send(ushort cmd, ByteBuffer buffer);

        void Dispatcher();

        void Disconnect();

        void UnRegister(ushort cmd);

        void Register(ushort cmd, ChannelMessageDelegate handler);
    }

    public abstract class AbsChannel : IChannel
    {
        protected Dictionary<ushort, ChannelMessageDelegate> handlers;

        protected AbsChannel()
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

        public void UnRegister(ushort cmd)
        {
            if (handlers.ContainsKey(cmd))
            {
                handlers.Remove(cmd);
            }
        }

        protected void ClearHandlers()
        {
            handlers.Clear();
        }

        public virtual void Dispose()
        {
            ClearHandlers();
        }

        public abstract void Dispatcher();

        public abstract void Disconnect();

        public abstract long ChannelId { get; }

        public abstract bool IsConnected { get; }

        public abstract void Send(ushort cmd, ByteBuffer buffer);
    }
}
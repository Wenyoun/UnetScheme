using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public delegate void ChannelMessageDelegate(ChannelMessage channelMessage);

    public struct ChannelMessage
    {
        public ushort Cmd;
        public ByteBuffer Buffer;

        public ChannelMessage(ushort cmd, ByteBuffer buffer)
        {
            Cmd = cmd;
            Buffer = buffer;
        }
    }

    public interface IChannel : IDisposable
    {
        uint ChannelId { get; }

        bool IsConnected { get; }

        void Send(ushort cmd, ByteBuffer buffer, byte channel);

        void Dispatcher();

        void Disconnect();

        void UnRegister(ushort cmd);

        void Register(ushort cmd, ChannelMessageDelegate handler);
    }

    public abstract class AbsChannel : IChannel
    {
        private Dictionary<ushort, ChannelMessageDelegate> m_Handlers;

        protected AbsChannel()
        {
            m_Handlers = new Dictionary<ushort, ChannelMessageDelegate>();
        }

        public void Register(ushort cmd, ChannelMessageDelegate handler)
        {
            if (!m_Handlers.ContainsKey(cmd))
            {
                m_Handlers.Add(cmd, handler);
            }
        }

        public void UnRegister(ushort cmd)
        {
            if (m_Handlers.ContainsKey(cmd))
            {
                m_Handlers.Remove(cmd);
            }
        }

        public virtual void Dispose()
        {
            ClearHandlers();
        }

        public abstract void Dispatcher();

        public abstract void Disconnect();

        public abstract uint ChannelId { get; }

        public abstract bool IsConnected { get; }

        public abstract void Send(ushort cmd, ByteBuffer buffer, byte channel);

        protected void ClearHandlers()
        {
            m_Handlers.Clear();
        }

        protected void Call(Packet packet)
        {
            if (m_Handlers.TryGetValue(packet.Cmd, out ChannelMessageDelegate handler))
            {
                handler.Invoke(new ChannelMessage(packet.Cmd, packet.Buffer));
            }
        }
    }
}
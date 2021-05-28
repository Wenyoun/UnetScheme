using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public delegate void ChannelMessageDelegate(ChannelMessage channelMessage);

    public struct ChannelMessage
    {
        public readonly ByteBuffer Buffer;

        public ChannelMessage(ByteBuffer buffer)
        {
            Buffer = buffer;
        }
    }

    public interface IChannel : IDisposable
    {
        uint ChannelId { get; }

        bool IsConnected { get; }

        void Send(ushort cmd, ByteBuffer buffer, ChannelType channel);

        void OnUpdate();

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
        }

        public abstract void OnUpdate();

        public abstract void Disconnect();

        public abstract uint ChannelId { get; }

        public abstract bool IsConnected { get; }

        public abstract void Send(ushort cmd, ByteBuffer buffer, ChannelType channel);

        protected void CallMsgHandler(ushort cmd, ByteBuffer buffer)
        {
            if (m_Handlers.TryGetValue(cmd, out ChannelMessageDelegate handler))
            {
                handler.Invoke(new ChannelMessage(buffer));
            }
        }

        protected void ClearMsgHandlers()
        {
            m_Handlers.Clear();
        }
    }
}
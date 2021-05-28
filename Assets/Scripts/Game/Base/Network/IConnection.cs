using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public interface IConnection : IDisposable
    {
        bool IsConnected { get; }

        uint ConnectionId { get; }

        void Disconnect();

        T RegisterProtocol<T>() where T : IProtocolHandler, new();

        void RegisterHandler(ushort cmd, ChannelMessageDelegate handler);

        void UnRegisterHandler(ushort cmd);

        void Send(ushort cmd, ByteBuffer buffer, ChannelType channel);
    }
}
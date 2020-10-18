﻿namespace Zyq.Game.Base
{
    public interface IServerCallback
    {
        void OnClientConnect(IChannel channel);

        void OnClientDisconnect(IChannel channel);
    }
}
﻿namespace Nice.Game.Base
{
    public interface IClientCallback
    {
        void OnServerConnect(IChannel channel);

        void OnServerDisconnect(IChannel channel);
    }
}
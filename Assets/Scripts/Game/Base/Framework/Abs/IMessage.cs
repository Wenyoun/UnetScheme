﻿namespace Nice.Game.Base
{
    public interface IMessage
    {
        void Dispatcher(int id, IBody body = null);
    }
}
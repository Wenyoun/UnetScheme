namespace Nice.Game.Base
{
    internal interface IClientListener
    {
        void OnTimeout(IChannel channel);

        void OnError(IChannel channel);

        void OnConnect(IChannel channel);

        void OnDisconnect(IChannel channel);
    }
}
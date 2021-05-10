namespace Nice.Game.Base {
    public interface IClientConnect {
        void OnTimeout(IChannel channel);

        void OnError(IChannel channel);

        void OnConnect(IChannel channel);

        void OnDisconnect(IChannel channel);
    }
}
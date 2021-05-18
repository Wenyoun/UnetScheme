namespace Nice.Game.Base {
    public interface IKcpConnect {
        void OnKcpConnect(IChannel channel);

        void OnKcpDisconnect(IChannel channel);
    }
}
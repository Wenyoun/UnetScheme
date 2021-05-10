namespace Nice.Game.Base
{
    public interface IServerConnect
    {
        void OnConnect(IChannel channel);

        void OnDisconnect(IChannel channel);
    }
}
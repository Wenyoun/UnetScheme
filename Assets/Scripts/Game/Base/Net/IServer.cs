namespace Zyq.Game.Base
{
    public interface IServer
    {
        void OnClientConnect(IChannel channel);

        void OnClientDisconnect(IChannel channel);
    }
}
namespace Zyq.Game.Base
{
    public interface IClient
    {
        void OnServerConnect(IChannel channel);

        void OnServerDisconnect(IChannel channel);
    }
}
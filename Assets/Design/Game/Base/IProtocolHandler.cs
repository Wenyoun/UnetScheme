namespace Zyq.Game.Base
{
    public interface IProtocolHandler
    {
        void Register(Connection connection);

        void Unregister(Connection connection);
    }
}
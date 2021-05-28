namespace Nice.Game.Base
{
    public interface IConnectionHandler
    {
        void OnAddConnection(IConnection connection);

        void OnRemoveConnection(IConnection connection);
    }
}
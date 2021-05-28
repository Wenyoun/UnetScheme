namespace Nice.Game.Base
{
    public interface IConnectionHandle
    {
        void OnAddConnection(IConnection connection);
        void OnRemoveConnection(IConnection connection);
    }
}
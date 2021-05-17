namespace Nice.Game.Base {
    public interface IClientHandler {
        void OnAddConnection(Connection connection);
        void OnRemoveConnection(Connection connection);
    }
}
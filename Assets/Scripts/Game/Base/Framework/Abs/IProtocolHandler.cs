namespace Nice.Game.Base {
    public interface IProtocolHandler {
        Connection Connection { set; }

        void Register();

        void UnRegister();
    }
}
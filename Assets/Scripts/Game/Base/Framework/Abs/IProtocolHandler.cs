namespace Nice.Game.Base
{
    public interface IProtocolHandler
    {
        IConnection Connection { set; }

        void Register();

        void UnRegister();
    }
}
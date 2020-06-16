namespace Zyq.Game.Base
{
    public interface IProtocolHandler
    {
        Connection Connection { get; set; }

        void Register();

        void Unregister();
    }
}
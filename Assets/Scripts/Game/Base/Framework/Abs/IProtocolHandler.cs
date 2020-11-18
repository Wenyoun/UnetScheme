namespace Zyq.Game.Base
{
    public interface IProtocolHandler
    {
        Connection Connection { set; }

        void Register();

        void Unregister();
    }
}
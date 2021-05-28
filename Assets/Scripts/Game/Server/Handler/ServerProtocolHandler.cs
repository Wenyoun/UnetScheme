using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class ServerProtocolHandler : IProtocolHandler
    {
        public IConnection Connection { get; set; }

        public void Register()
        {
        }

        public void UnRegister()
        {
        }
    }
}
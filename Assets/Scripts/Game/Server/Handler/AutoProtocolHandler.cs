using Nice.Game.Base;

namespace Nice.Game.Server
{
    [Protocol]
    public class AutoProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }

        public void Register()
        {
        }

        public void UnRegister()
        {
        }
    }
}
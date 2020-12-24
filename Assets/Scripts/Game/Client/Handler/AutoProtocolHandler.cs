using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    [Protocol]
    public class AutoProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }

        public void Register() { }

        public void UnRegister() { }
    }
}
using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    [Protocol]
    public class AutoProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }
        public void Register() { }
        public void UnRegister() { }
    }
}
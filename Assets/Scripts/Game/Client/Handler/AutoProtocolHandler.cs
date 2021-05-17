using Nice.Game.Base;

namespace Nice.Game.Client {
    [Protocol]
    public class AutoProtocolHandler : IProtocolHandler {
        public Connection Connection { get; set; }

        public void Register() {
        }

        public void UnRegister() {
        }
    }
}
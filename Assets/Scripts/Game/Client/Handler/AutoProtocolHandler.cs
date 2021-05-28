using Nice.Game.Base;

namespace Nice.Game.Client
{
    [Protocol]
    public class AutoProtocolHandler : IProtocolHandler
    {
        public IConnection Connection { get; set; }

        public void Register()
        {
        }

        public void UnRegister()
        {
        }

        public void Register111111111()
        {
            Connection.RegisterHandler((ushort) 2, OnProtocol_222222222222);
        }

        public void UnRegister11111111111()
        {
            Connection.UnRegisterHandler((ushort) 2);
        }

        private void OnProtocol_222222222222(ChannelMessage msg)
        {
        }
    }
}
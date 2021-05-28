using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class InitServerLogic : AbsServerLogic, IConnectionHandler
    {
        protected override void Init()
        {
            NetworkServerManager.Bind(50000, this);
        }

        protected override void Clear()
        {
            NetworkServerManager.Dispose();
        }

        public void OnAddConnection(IConnection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ServerProtocolHandler>();
        }

        public void OnRemoveConnection(IConnection connection)
        {
            connection.Disconnect();
        }
    }
}
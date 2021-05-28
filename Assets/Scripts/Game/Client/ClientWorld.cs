using Nice.Game.Base;

namespace Nice.Game.Client
{
	public class ClientWorld : World, IConnectionHandle
	{
		public ClientWorld(int wid) : base(wid)
		{
		}

		protected override void Init()
		{
			AddFeature<ConnectFeature>();
			NetworkClientManager.Connect("127.0.0.1", 50000, this);
		}

		protected override void Clear()
		{
			NetworkClientManager.Disconnect();
		}

		public void OnAddConnection(IConnection connection)
		{
			DispatchMessage(MessageConstants.AddConnection, connection);
		}

		public void OnRemoveConnection(IConnection connection)
		{
			DispatchMessage(MessageConstants.RemoveConnection, connection);
		}
	}
}
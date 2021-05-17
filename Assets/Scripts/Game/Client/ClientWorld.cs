using Nice.Game.Base;

namespace Nice.Game.Client {
	public class ClientWorld : World, IClientHandler {
		public ClientWorld(int wid) : base(wid) {
		}

		protected override void Init() {
			AddFeature<ConnectFeature>();
			NetworkClientManager.Start("127.0.0.1", 50000, this);
		}

		protected override void Clear() {
			NetworkClientManager.Disconnect();
		}

		public void OnAddConnection(Connection connection) {
			DispatchMessage(MessageConstants.AddConnection, connection);
		}

		public void OnRemoveConnection(Connection connection) {
			DispatchMessage(MessageConstants.RemoveConnection, connection);
		}
	}
}
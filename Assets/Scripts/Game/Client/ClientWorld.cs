using Nice.Game.Base;

namespace Nice.Game.Client {
	public class ClientWorld : World, IClientHandler {
		public ClientWorld() : base(1) {
		}

		protected override void Init() {
			AddFeature<ConnectFeature>();
		}

		public void OnAddConnection(Connection connection) {
			DispatchMessage<Connection>(MessageConstants.Connect, connection);
		}

		public void OnRemoveConnection(Connection connection) {
			DispatchMessage<Connection>(MessageConstants.Disconnect, connection);
		}
	}
}
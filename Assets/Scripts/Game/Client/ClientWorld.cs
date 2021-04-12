using Nice.Game.Base;

namespace Nice.Game.Client
{
	public class ClientWorld : World
	{
		public ClientWorld() : base(1)
		{
		}

		protected override void Init()
		{
			AddFeature<ConnectFeature>();
		}
	}
}
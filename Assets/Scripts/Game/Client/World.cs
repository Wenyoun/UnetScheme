using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Client
{
	public class World : AbsWorld
	{
		public World() : base(1)
		{
		}

		protected override void Init()
		{
			LogicManager.AddLogic<ConnectLogic>();
		}

		protected override void Clear()
		{
		}

		public override void OnUpdate(float delta)
		{
		}
	}
}
using System;

namespace Nice.Game.Base
{
	public interface IWorld : IDisposable, IUpdate, ILateUpdate, IFixedUpdate
	{
		int Wid { get; }

		void OnInit();

		void AddEntity(Entity entity);

		void RemoveEntity(uint entityId);

		Entity GetEntity(uint entityId);

		void Dispatcher(int msgId, IBody body);

		Entities Entities { get; }

		TimerRegister Timer { get; }

		UpdaterRegister Updater { get; }

		MessagerRegister Messager { get; }

		WorldLogicManager LogicManager { get; }
	}
}
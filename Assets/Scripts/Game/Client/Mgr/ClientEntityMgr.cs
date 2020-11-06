using Zyq.Game.Base;

namespace Zyq.Game.Client
{

	public class ClientEntityMgr
	{
		private Entities m_Entities;

		public ClientEntityMgr()
		{
			m_Entities = new Entities();
		}

		public void Dispose()
		{
			m_Entities.Dispose();
		}

		public void AddEntity(Entity entity)
		{
			m_Entities.AddEntity(entity);
		}

		public void RemoveEntity(uint eid)
		{
			m_Entities.RemoveEntity(eid);
		}

		public Entity GetEntity(uint eid)
		{
			return m_Entities.GetEntity(eid);
		}

		public void Dispatcher(int msgId)
		{
			m_Entities.Dispatcher(msgId, 0, null);
		}

		public void Dispatcher(int msgId, uint entityId)
		{
			m_Entities.Dispatcher(msgId, entityId, null);
		}

		public void Dispatcher(int msgId, IBody body)
		{
			m_Entities.Dispatcher(msgId, 0, body);
		}

		public void Dispatcher(int msgId, uint eid, IBody body)
		{
			m_Entities.Dispatcher(msgId, eid, body);
		}

		public void OnUpdate(float delta)
		{
			m_Entities.OnUpdate(delta);
		}

		public void OnFixedUpdate(float delta)
		{
			m_Entities.OnFixedUpdate(delta);
		}
	}
}
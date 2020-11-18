namespace Zyq.Game.Base
{
	public abstract class AbsWorld : IWorld
	{
		protected int m_WorldId;
		protected Entities m_Entities;
		protected TimerMgr m_TimerMgr;
		protected UpdateMgr m_UpdateMgr;
		protected MessageMgr m_MessageMgr;
		protected WorldLogicMgr m_WorldLogicMgr;

		public AbsWorld(int worldId)
		{
			m_WorldId = worldId;
			m_TimerMgr = new TimerMgr();
			m_UpdateMgr = new UpdateMgr();
			m_MessageMgr = new MessageMgr();
			m_WorldLogicMgr = new WorldLogicMgr(this);
			m_Entities = new Entities(this);
		}

		public virtual void OnInit()
		{
		}

		public virtual void Dispose()
		{
			m_WorldLogicMgr.Dispose();
			m_TimerMgr.Dispose();
			m_UpdateMgr.Dispose();
			m_MessageMgr.Dispose();
			m_Entities.Dispose();
		}

		public void AddWorldLogic<T>() where T : IWorldLogic, new()
		{
			m_WorldLogicMgr.AddLogic<T>();
		}

		public void RemoveWorldLogic<T>() where T : IWorldLogic, new()
		{
			m_WorldLogicMgr.RemoveLogic<T>();
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

		public virtual void OnUpdate(float delta)
		{
			m_TimerMgr.OnUpdate(delta);
			m_UpdateMgr.OnUpdate(delta);
			m_Entities.OnUpdate(delta);
		}

		public virtual void OnFixedUpdate(float delta)
		{
			m_Entities.OnFixedUpdate(delta);
		}

		public virtual void OnLateUpdate()
		{
			m_UpdateMgr.OnLateUpdate();
		}

		public int WorldId
		{
			get { return m_WorldId; }
		}

		public TimerMgr Timer
		{
			get { return m_TimerMgr; }
		}

		public UpdateMgr Update
		{
			get { return m_UpdateMgr; }
		}

		public MessageMgr Message
		{
			get { return m_MessageMgr; }
		}
	}
}
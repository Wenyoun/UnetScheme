namespace Nice.Game.Base
{
	public abstract class AbsWorld : IWorld
	{
		private int m_Wid;
		private Entities m_Entities;
		private TimerRegister m_Timer;
		private UpdaterRegister m_Updater;
		private MessagerRegister m_Messager;
		private WorldLogicManager m_LogicManager;

		public AbsWorld(int wid)
		{
			m_Wid = wid;
			m_Timer = new TimerRegister();
			m_Updater = new UpdaterRegister();
			m_Messager = new MessagerRegister();
			m_LogicManager = new WorldLogicManager(this);
			m_Entities = new Entities(this);
		}

		public void OnInit()
		{
			Init();
		}

		public void Dispose()
		{
			Clear();
			m_Timer.Dispose();
			m_Updater.Dispose();
			m_Messager.Dispose();
			m_Entities.Dispose();
			m_LogicManager.Dispose();
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

		public void Dispatcher(int msgId, IBody body = null)
		{
			m_Messager.Dispatcher(msgId, body);
		}

		public virtual void OnUpdate(float delta)
		{
			m_Timer.OnUpdate(delta);
			m_Updater.OnUpdate(delta);
			m_Messager.OnUpdate(delta);
			m_Entities.OnUpdate(delta);
		}

		public virtual void OnFixedUpdate(float delta)
		{
			m_Updater.OnFixedUpdate(delta);
			m_Entities.OnFixedUpdate(delta);
		}

		public virtual void OnLateUpdate()
		{
		}

		public int Wid
		{
			get { return m_Wid; }
		}

		public Entities Entities
		{
			get { return m_Entities; }
		}

		public TimerRegister Timer
		{
			get { return m_Timer; }
		}

		public UpdaterRegister Updater
		{
			get { return m_Updater; }
		}

		public MessagerRegister Messager
		{
			get { return m_Messager; }
		}

		public WorldLogicManager LogicManager
		{
			get { return m_LogicManager; }
		}

		protected abstract void Init();

		protected abstract void Clear();
	}
}
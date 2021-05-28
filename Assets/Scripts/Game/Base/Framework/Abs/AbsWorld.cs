namespace Nice.Game.Base
{
	public abstract partial class AbsWorld
	{
		private int m_Wid;
		private bool m_Dispose;
		private Entities m_Entities;
		private TimerRegister m_Timer;
		private UpdaterRegister m_Updater;
		private UpdaterRegister m_LateUpdater;
		private UpdaterRegister m_FixedUpdater;
		private MessagerRegister m_Messager;
		private FeatureRegister m_Features;

		protected AbsWorld(int wid)
		{
			m_Wid = wid;
		}

		public void OnInit()
		{
			m_Dispose = false;
			m_Timer = new TimerRegister();
			m_Updater = new UpdaterRegister();
			m_LateUpdater = new UpdaterRegister();
			m_FixedUpdater = new UpdaterRegister();
			m_Messager = new MessagerRegister();
			m_Features = new FeatureRegister(this);
			m_Entities = new Entities(this);
			Init();
		}

		public void Dispose()
		{
			if (m_Dispose)
			{
				return;
			}
			Clear();
			m_Dispose = true;
			m_Timer.Dispose();
			m_Updater.Dispose();
			m_Messager.Dispose();
			m_Entities.Dispose();
			m_Features.Dispose();
		}

		public void OnUpdate(float delta)
		{
			if (m_Dispose)
			{
				return;
			}
			m_Timer.OnUpdate(delta);
			m_Messager.OnUpdate(delta);
			m_Updater.OnUpdate(delta);
			m_Entities.OnUpdate(delta);
		}

		public void OnFixedUpdate(float delta)
		{
			if (m_Dispose)
			{
				return;
			}
			m_FixedUpdater.OnUpdate(delta);
		}

		public void OnLateUpdate(float delta)
		{
			if (m_Dispose)
			{
				return;
			}
			m_LateUpdater.OnUpdate(delta);
		}

		public int Wid
		{
			get { return m_Wid; }
		}

		protected virtual void Init()
		{
		}

		protected virtual void Clear()
		{
		}
	}
}
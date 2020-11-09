using Zyq.Game.Base;

namespace Zyq.Game.Client
{
	public class World : IClientCallback, IWorld
	{
		private int m_WorldId;
		private Entities m_Entities;
		private TimerMgr m_TimerMgr;
		private UpdateMgr m_UpdateMgr;
		private MessageMgr m_MessageMgr;
		private Connection m_Connection;

		public World()
		{
			m_WorldId = 1;
			m_TimerMgr = new TimerMgr();
			m_UpdateMgr = new UpdateMgr();
			m_MessageMgr = new MessageMgr();
			m_Entities = new Entities(this);
		}

		public void Dispose()
		{
			m_TimerMgr.Dispose();
			m_UpdateMgr.Dispose();
			m_MessageMgr.Dispose();
			m_Entities.Dispose();
			m_Connection?.Dispose();
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
			m_Connection?.Dispatcher();
			m_TimerMgr.OnUpdate(delta);
			m_UpdateMgr.OnUpdate(delta);
			m_Entities.OnUpdate(delta);
		}

		public void OnFixedUpdate(float delta)
		{
			m_Entities.OnFixedUpdate(delta);
		}

		public void OnLateUpdate()
		{
			m_UpdateMgr.OnLateUpdate();
		}

		public void Connect(string host, int port)
		{
			ClientChannel channel = new ClientChannel(this);
			channel.Connect(host, port);
		}

		public void Send(ushort cmd, ByteBuffer buffer)
		{
			m_Connection?.Send(cmd, buffer);
		}

		public TimerMgr WorldTimer
		{
			get { return m_TimerMgr; }
		}

		public UpdateMgr WorldUpdate
		{
			get { return m_UpdateMgr; }
		}

		public MessageMgr WorldMessage
		{
			get { return m_MessageMgr; }
		}

		public int WorldId
		{
			get { return m_WorldId; }
		}

		public Connection Connection
		{
			get { return m_Connection; }
		}

		public void OnServerConnect(IChannel channel)
		{
			if (m_Connection == null)
			{
				m_Connection = new Connection(channel);
				RegisterProtocols(m_Connection);
			}
		}

		public void OnServerDisconnect(IChannel channel)
		{
			m_Connection?.Dispose();
		}

		private void RegisterProtocols(Connection connection)
		{
			connection.RegisterProtocol<AutoProtocolHandler>();
			connection.RegisterProtocol<ClientProtocolHandler>();
		}
	}
}
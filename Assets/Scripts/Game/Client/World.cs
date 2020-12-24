using Zyq.Game.Base;

namespace Zyq.Game.Client
{
	public class World : AbsWorld, IClientCallback
	{
		private Connection m_Connection;
		private ClientChannel m_Channel;

		public World() : base(1)
		{
			m_Channel = new ClientChannel(this);
		}

		protected override void Init()
		{
			LogicManager.AddLogic<ConnectLogic>();
		}

		protected override void Clear()
		{
			m_Channel.Dispose();
			m_Connection?.Dispose();
		}

		public override void OnUpdate(float delta)
		{
			m_Channel?.Dispatcher();
		}

		public void Connect(string host, int port)
		{
			m_Channel.Connect(host, port);
		}

		public void Send(ushort cmd, ByteBuffer buffer)
		{
			m_Connection?.Send(cmd, buffer);
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
				Messager.Dispatcher(MessageConstants.Connect_Success);
			}
		}

		public void OnServerDisconnect(IChannel channel)
		{
			m_Connection?.Dispose();
			Messager.Dispatcher(MessageConstants.Connect_Error);
		}

		private void RegisterProtocols(Connection connection)
		{
			connection.RegisterProtocol<AutoProtocolHandler>();
			connection.RegisterProtocol<ClientProtocolHandler>().World = this;
		}
	}
}
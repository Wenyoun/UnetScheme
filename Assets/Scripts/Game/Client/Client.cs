using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class Client : AbsMachine, IClientCallback
    {
        public static Client Ins = new Client();

        #region Fields

        private Connection m_Connection;
        private ClientEntityMgr m_EntityMgr;

        #endregion

        #region Properties

        public Connection Connection => m_Connection;
        public ClientEntityMgr EntityMgr => m_EntityMgr;

        #endregion

        private Client()
        {
        }

        public override void OnInit()
        {
            base.OnInit();
            m_EntityMgr = new ClientEntityMgr();
            for (int i = 0; i < 1; ++i)
            {
                ClientNetworkMgr.Connect("127.0.0.1", 50000, this);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            m_EntityMgr.Dispose();
            ClientNetworkMgr.Dispose();
        }

        public void OnStartClient()
        {
        }

        public void OnStopClient()
        {
        }

        public void Send(ushort cmd, ByteBuffer buffer)
        {
            if (m_Connection != null)
            {
                m_Connection.Send(cmd, buffer);
            }
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            ClientNetworkMgr.Dispatcher();

            if (m_EntityMgr != null)
            {
                m_EntityMgr.OnUpdate(delta);
            }
        }

        public override void OnFixedUpdate(float delta)
        {
            base.OnFixedUpdate(delta);
            if (m_EntityMgr != null)
            {
                m_EntityMgr.OnFixedUpdate(delta);
            }
        }

        public void OnServerConnect(IChannel channel)
        {
            if (m_Connection == null)
            {
                m_Connection = new Connection();
            }

            Debug.Log("Client OnServerConnect:" + channel.ChannelId);

            m_Connection.OnConnect(channel);
            RegisterProtocols(m_Connection);
        }

        public void OnServerDisconnect(IChannel channel)
        {
            Debug.Log("Client OnServerDisconnect:" + channel.ChannelId);
            
            if (m_Connection != null)
            {
                m_Connection.OnDisconnect(channel);
            }
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
        }
    }
}
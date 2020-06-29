using UnityEngine;
using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class Client : AbsMachine
    {
        public static Client Ins = new Client();

        #region Fields
        public Connection m_Connection;
        public ClientEntityMgr m_EntityMgr;
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
            m_Connection = null;
            m_EntityMgr = new ClientEntityMgr();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            m_EntityMgr.Dispose();
            m_EntityMgr = null;
            m_Connection.Dispose();
            m_Connection = null;
        }

        public void OnStartClient()
        {
        }

        public void OnStopClient()
        {
        }

        public void Send(NetworkWriter writer)
        {
            if (m_Connection != null)
            {
                m_Connection.Send(writer);
            }
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
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

        public override void OnNetConnect(NetworkConnection network)
        {
            if (m_Connection == null)
            {
                m_Connection = new Connection();
            }
            m_Connection.OnConnect(network);
            RegisterProtocols(m_Connection);
            Sender.RpcLogin(1, true, 2, 3, 4, 5, 6, 7, 8, 9, "yinhuayong", Vector2.zero, Vector3.zero, Vector4.zero, Quaternion.identity);
        }

        public override void OnNetDisconnect(NetworkConnection network)
        {
            if (m_Connection != null)
            {
                m_Connection.OnDisconnect(network);
            }
        }

        private void RegisterProtocols(Connection connection)
        {
            connection.RegisterProtocol<AutoProtocolHandler>();
            connection.RegisterProtocol<ClientProtocolHandler>();
        }
    }
}
﻿using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class Client : AbsMachine, IClient
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
        }

        public override void OnRemove()
        {
            base.OnRemove();
            ClientNetworkMgr.Dispose();
            m_EntityMgr.Dispose();
            m_Connection.Dispose();
            m_EntityMgr = null;
            m_Connection = null;
        }

        public void OnStartClient()
        {
            for (int i = 0; i < 1; ++i)
            {
                ClientNetworkMgr.Connect("127.0.0.1", 50000, this);
            }
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

        public override void OnNetConnect(NetworkConnection network)
        {
            if (m_Connection == null)
            {
                m_Connection = new Connection();
            }

            m_Connection.OnConnect(network);
            RegisterProtocols(m_Connection);

            Login[] logins = new Login[3];
            logins[0] = Login.Log1;
            logins[1] = Login.Log3;
            logins[2] = Login.Log5;

            LoginData data = new LoginData();
            data.Username = "Username";
            data.Password = "Password";

            LoginData[] datas = new LoginData[3];
            datas[0].Username = "Username0";
            datas[0].Password = "Password0";
            datas[1].Username = "Username1";
            datas[1].Password = "Password1";
            datas[2].Username = "Username2";
            datas[2].Password = "Password2";

            ClientSender.RpcLogin(1, true, 2, 3, 4, 5, 6, 7, 8, 9, "yinhuayong", Vector2.zero, Vector3.zero,
                Vector4.zero, Quaternion.identity, Login.Log5, logins, data, datas);
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

        public void OnServerConnect(IChannel channel)
        {
            Debug.Log("Client:1111111111111111111:" + channel.ChannelId);
            Packet packet = new Packet();
            packet.Cmd = 1;
            packet.Buffer = ByteBuffer.Allocate(10240);
            string k = "";
            for (int i = 0; i < 1000; ++i)
            {
                k += "yinhuayong";
            }
            packet.Buffer.Write(k);
            channel.Send(packet);
        }

        public void OnServerDisconnect(IChannel channel)
        {
            Debug.Log("Server:2222222222222222222");
        }
    }
}
﻿using Base;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace System
{
    public class LobbyView : AbsView
    {
        private Button m_CreateServer;
        private Button m_CreateHost;
        private Button m_JoinServer;
        private InputField m_Host;
        private InputField m_Port;

        public LobbyView(GameObject root)
        : base(root)
        {
        }

        public override void OnInit()
        {
            m_CreateHost = Cop.Get<Button>("0");
            m_JoinServer = Cop.Get<Button>("1");
            m_CreateServer = Cop.Get<Button>("2");
            m_Host = Cop.Get<InputField>("3");
            m_Port = Cop.Get<InputField>("4");
        }

        private NetworkManager m_Network;

        public override void OnRegEvent()
        {
            m_CreateServer.onClick.AddListener(() =>
            {
                if (m_Network == null)
                {
                    m_Network = SimpleResMgr.CreateRoot("Prefabs/Game/ServerNetworkManager").GetComponent<NetworkManager>();
                    m_Network.StartServer();
                }
            });

            m_CreateHost.onClick.AddListener(() =>
            {
                if (m_Network == null)
                {
                    m_Network = SimpleResMgr.CreateRoot("Prefabs/Game/HostNetworkManager").GetComponent<NetworkManager>();
                    m_Network.StartHost();
                }
            });

            m_JoinServer.onClick.AddListener(() =>
            {
                if (m_Network == null)
                {
                    m_Network = SimpleResMgr.CreateRoot("Prefabs/Game/ClientNetworkManager").GetComponent<NetworkManager>();
                    m_Network.StartClient();
                }
            });
        }
    }
}
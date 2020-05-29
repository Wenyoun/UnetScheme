using Base;
using UnityEngine;
using UnityEngine.UI;
using Zyq.Game.Host;
using Zyq.Game.Client;
using Zyq.Game.Server;

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

        public override void OnRegEvent()
        {
            m_CreateServer.onClick.AddListener(() =>
            {
                if (ServerNetworkManager.Ins == null)
                {
                    SimpleResMgr.CreateRoot("Prefabs/Game/ServerNetworkManager");
                    ServerNetworkManager.Ins.StartServer();
                }
            });

            m_CreateHost.onClick.AddListener(() =>
            {
                if (HostNetworkManager.Ins == null)
                {
                    SimpleResMgr.CreateRoot("Prefabs/Game/HostNetworkManager");
                    HostNetworkManager.Ins.StartHost();
                }
            });

            m_JoinServer.onClick.AddListener(() =>
            {
                if (ClientNetworkManager.Ins == null)
                {
                    SimpleResMgr.CreateRoot("Prefabs/Game/ClientNetworkManager");
                    ClientNetworkManager.Ins.StartClient();
                }
            });
        }
    }
}
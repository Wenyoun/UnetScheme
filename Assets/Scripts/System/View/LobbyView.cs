using Base;
using UnityEngine;
using UnityEngine.UI;
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
                GameMgr.Ins.Add<Server>();
            });

            m_CreateHost.onClick.AddListener(() =>
            {
                GameMgr.Ins.Add<Server>();
                GameMgr.Ins.Add<Client>();
            });

            m_JoinServer.onClick.AddListener(() =>
            {
                GameMgr.Ins.Add<Client>();
            });
        }
    }
}
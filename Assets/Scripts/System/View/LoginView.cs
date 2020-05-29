using Base;
using UnityEngine;
using UnityEngine.UI;

namespace System
{
    public class LoginView : AbsView
    {
        private Button m_Login;

        public LoginView(GameObject root)
        : base(root)
        {
        }

        public override void OnInit()
        {
            m_Login = Cop.Get<Button>("1");
        }

        public override void OnRegEvent()
        {
            m_Login.onClick.AddListener(Login);
        }

        private void Login()
        {
            PageMgr.Ins.Push<LobbyPage>();
        }
    }
}
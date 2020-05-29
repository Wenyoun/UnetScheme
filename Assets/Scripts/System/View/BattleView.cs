using Base;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace System
{
    public class BattleView : AbsView
    {
        private Button m_ExitBattle;

        public BattleView(GameObject root)
        : base(root)
        {
        }

        public override void OnInit()
        {
            m_ExitBattle = Cop.Get<Button>("0");
        }

        public override void OnRegEvent()
        {
            m_ExitBattle.onClick.AddListener(() =>
            {
            });
        }
    }
}
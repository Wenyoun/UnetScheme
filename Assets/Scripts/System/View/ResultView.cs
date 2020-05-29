using Base;
using UnityEngine;

namespace System
{
    public class ResultView : AbsView
    {
        private UnityEngine.UI.Text m_Result;

        public ResultView(GameObject root)
        : base(root)
        {
        }

        public override void OnInit()
        {
            m_Result = Cop.Get<UnityEngine.UI.Text>("0");
        }

        public override void OnShow(object body)
        {
            m_Result.text = body.ToString();
            base.OnShow(body);
        }
    }
}
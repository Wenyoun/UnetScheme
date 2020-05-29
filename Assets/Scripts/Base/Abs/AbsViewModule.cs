using UnityEngine;
using System.Collections.Generic;

namespace Base
{
    public abstract class AbsViewModule : AbsCompose
    {
        private ViewMgr mViewMgr;
        private List<System.Type> mProxys;
        private List<string> mMediators;

        public AbsViewModule()
        {
            mViewMgr = new ViewMgr();
            mProxys = new List<System.Type>();
            mMediators = new List<string>();
            Root = CreateRoot();
            BRoot = CreateBRoot();
            NRoot = CreateNRoot();
            MRoot = CreateMRoot();
            PRoot = CreatePRoot();
        }


        public GameObject Root { protected set; get; }

        public GameObject BRoot { protected set; get; }

        public GameObject NRoot { protected set; get; }

        public GameObject MRoot { protected set; get; }

        public GameObject PRoot { protected set; get; }


        public override void OnInit()
        {
            RegProxys();
            RegMediators();
        }

        public override void OnRemove()
        {
            foreach (System.Type type in mProxys)
            {
                Facade.Ins.RemoveProxy(type);
            }
            foreach (string name in mMediators)
            {
                Facade.Ins.RemoveMediator(name);
            }
            mProxys.Clear();
            mMediators.Clear();
            mViewMgr.Dispose();
        }

        public override void OnUpdate(float delta)
        {
            mViewMgr.OnUpdate(delta);
        }

        public abstract GameObject CreateRoot();

        public virtual GameObject CreateBRoot()
        {
            return Root.transform.Find("BRoot").gameObject;
        }

        public virtual GameObject CreateNRoot()
        {
            return Root.transform.Find("NRoot").gameObject;
        }

        public virtual GameObject CreateMRoot()
        {
            return Root.transform.Find("MRoot").gameObject;
        }

        public virtual GameObject CreatePRoot()
        {
            return Root.transform.Find("PRoot").gameObject;
        }

        public virtual void RegProxys()
        {
        }

        public virtual void RegMediators()
        {
        }

        public void Show(int id, object body)
        {
            mViewMgr.Show(id, body);
        }

        public void Hide(int id, object body)
        {
            mViewMgr.Hide(id, body);
        }

        public void RegView(int id, AbsView view)
        {
            mViewMgr.RegView(id, view);
        }

        protected void RegMediator(AbsViewMediator mediator)
        {
            if (!mMediators.Contains(mediator.Name))
            {
                mMediators.Add(mediator.Name);
                Facade.Ins.RegisterMediator(mediator);
            }
        }

        protected void RegProxy(Proxy proxy)
        {
            if (!mProxys.Contains(proxy.GetType()))
            {
                mProxys.Add(proxy.GetType());
                Facade.Ins.RegisterProxy(proxy);
            }
        }
    }
}
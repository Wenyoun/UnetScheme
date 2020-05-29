namespace Base
{
    public abstract class AbsViewMediator: AbsMediator
    {
        private string mShow;
        private string mHide;
        private AbsViewModule mModule;
        protected AbsView mView;

        public AbsViewMediator(string name, string show, string hide, AbsViewModule module, bool now = false)
            : base(name)
        {
            mShow = show;
            mHide = hide;
            mModule = module;
            if (now)
            {
                CreateView();
            }
        }

        public override void OnRegister()
        {
            RegHandler(mShow, (object body) => { mModule.Show(mView.Id, body); });
            RegHandler(mHide, (object body) => { mModule.Hide(mView.Id, body); });
        }

        public override void OnRemove()
        {
            base.OnRemove();
            mShow = null;
            mHide = null;
            mView = null;
            mModule = null;
        }

        public override void Notify(string notificationName, object body)
        {
            if (mView == null)
            {
                CreateView();
            }
            base.Notify(notificationName, body);
        }

        private void CreateView()
        {
            if (mView == null)
            {
                mView = CreateView(mModule);
                mModule.RegView(mView.Id, mView);
            }
        }

        public abstract AbsView CreateView(AbsViewModule module);
    }
}
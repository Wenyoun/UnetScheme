using System.Collections.Generic;

namespace Base
{
    public abstract class AbsMediator : Mediator
    {
        private Dictionary<string, System.Action<object>> mHandlers;

        public AbsMediator(string name)
            : base(name)
        {
            mHandlers = new Dictionary<string, System.Action<object>>();
        }

        public override void OnRegister()
        {
        }

        public override void OnRemove()
        {
            mHandlers.Clear();
            mHandlers = null;
        }

        public override List<string> Notifys()
        {
            List<string> lists = new List<string>();
            foreach (string key in mHandlers.Keys)
            {
                lists.Add(key);
            }
            return lists;
        }

        public override void Notify(string name, object body)
        {
            System.Action<object> handler = null;
            if (mHandlers.TryGetValue(name, out handler))
            {
                handler(body);
            }
        }

        protected void RegHandler(string name, System.Action<object> handler)
        {
            if (!mHandlers.ContainsKey(name))
            {
                mHandlers.Add(name, handler);
            }
        }

        protected void UnRegHandler(string name)
        {
            if (mHandlers.ContainsKey(name))
            {
                mHandlers.Remove(name);
            }
        }
    }
}
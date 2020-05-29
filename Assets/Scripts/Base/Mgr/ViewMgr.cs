using System;
using System.Collections.Generic;

namespace Base
{
    public class ViewMgr : IDisposable, IUpdate
    {
        private Dictionary<int, int> mStartDys;
        private Dictionary<int, AbsView> mViewDys;
        private Dictionary<int, AbsView> mVisibleDys;

        public ViewMgr()
        {
            mStartDys = new Dictionary<int, int>();
            mViewDys = new Dictionary<int, AbsView>();
            mVisibleDys = new Dictionary<int, AbsView>();
        }

        public void OnUpdate(float delta)
        {
            Dictionary<int, AbsView>.Enumerator enumerator = mViewDys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AbsView view = enumerator.Current.Value;
                if (view.IsUpdate)
                {
                    view.OnUpdate(delta);
                }
            }
        }

        public void Show(int id, object body)
        {
            AbsView view = null;
            if (mViewDys.TryGetValue(id, out view))
            {
                if (IsHide(id))
                {
                    mVisibleDys.Add(id, view);
                    if (!mStartDys.ContainsKey(id))
                    {
                        mStartDys.Add(id, id);
                        view.OnStart();
                    }
                    view.OnShowBefore();
                    view.OnShow(body);
                    view.Root.transform.SetAsLastSibling();
                    view.IsVisible = true;
                }
                else
                {
                    view.OnRepeat(body);
                }
            }
        }

        public void Hide(int id, object body)
        {
            if (IsShow(id))
            {
                AbsView view = null;
                if (mViewDys.TryGetValue(id, out view))
                {
                    mVisibleDys.Remove(id);
                    view.OnHideBefore();
                    view.OnHide(body);
                    view.IsVisible = false;
                }
            }
        }

        public void RegView(int id, AbsView view)
        {
            if (!mViewDys.ContainsKey(id))
            {
                mViewDys.Add(id, view);
                view.OnInit();
                view.OnRegEvent();
            }
        }

        public void UnRegView(int id)
        {
            AbsView view = null;
            if (mViewDys.TryGetValue(id, out view))
            {
                mViewDys.Remove(id);
                mVisibleDys.Remove(id);
                view.OnRemove();
            }
        }

        public void Dispose()
        {
            foreach (AbsView view in mViewDys.Values)
            {
                view.OnRemove();
            }
            mViewDys.Clear();
            mStartDys.Clear();
            mVisibleDys.Clear();
        }

        private bool IsShow(int id)
        {
            return mVisibleDys.ContainsKey(id);
        }

        private bool IsHide(int id)
        {
            return !mVisibleDys.ContainsKey(id);
        }
    }
}
using System.Collections.Generic;

namespace Base
{
    public class PageMgr
    {
        public static PageMgr Ins = new PageMgr();

        private Stack<IPage> m_Pages;

        public PageMgr()
        {
            m_Pages = new Stack<IPage>();
        }

        public void Push<T>(object body = null) where T : IPage, new()
        {
            IPage page = Pool.Get<T>();

            if (m_Pages.Count == 0)
            {
                m_Pages.Push(page);
                page.Show(body);
            }
            else if (m_Pages.Count > 0)
            {
                if (Contains(page))
                {
                    if (m_Pages.Peek().GetType() == page.GetType())
                    {
                        return;
                    }

                    m_Pages.Pop().Hide(body);

                    while (m_Pages.Count > 0)
                    {
                        IPage old = m_Pages.Pop();
                        if (old.GetType() == page.GetType())
                        {
                            m_Pages.Push(page);
                            page.Show(body);
                            return;
                        }
                    }
                }
                else
                {
                    IPage old = m_Pages.Peek();
                    if (page.GetType() != old.GetType())
                    {
                        m_Pages.Push(page);
                        old.Hide(body);
                        page.Show(body);
                    }
                }
            }
        }

        public void Pop(object body = null)
        {
            if (m_Pages.Count > 1)
            {
                m_Pages.Pop().Hide(body);
                m_Pages.Peek().Show(body);
            }
        }

        private bool Contains(IPage page)
        {
            foreach (IPage p in m_Pages)
            {
                if (p.GetType() == page.GetType())
                {
                    return true;
                }
            }
            return false;
        }

        private class Pool
        {
            private static Dictionary<System.Type, IPage> VALUES = new Dictionary<System.Type, IPage>();

            public static IPage Get<T>() where T : IPage, new()
            {
                IPage page = null;
                System.Type t = typeof(T);
                if (!VALUES.TryGetValue(t, out page))
                {
                    page = new T();
                    VALUES.Add(t, page);
                }
                return page;
            }
        }
    }
}
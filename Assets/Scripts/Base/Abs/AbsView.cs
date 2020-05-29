using UnityEngine;

namespace Base
{
    public abstract class AbsView : IView
    {
        private static IdGenerator GEN = new IdGenerator();

        public AbsView(GameObject root)
        {
            Id = GEN.Next;
            Root = root;
            Cop = root.GetComponent<ViewComponent>();
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnInit()
        {
        }

        public virtual void OnRegEvent()
        {
        }

        public virtual void OnRemove()
        {
            if (Root != null)
            {
                Object.Destroy(Root);
                Root = null;
            }
        }

        public virtual void OnUpdate(float delta)
        {
        }

        public virtual void OnShowBefore()
        {
        }

        public virtual void OnShow(object body)
        {
            if (Root != null && !Root.activeInHierarchy)
            {
                Root.SetActive(true);
            }
        }

        public virtual void OnHideBefore()
        {
        }

        public virtual void OnHide(object body)
        {
            if (Root != null && Root.activeInHierarchy)
            {
                Root.SetActive(false);
            }

        }

        public virtual void OnRepeat(object body)
        {
        }

        public int Id { get; private set; }

        public bool IsUpdate { get; set; }

        public bool IsVisible { get; set; }

        public GameObject Root { get; private set; }

        public ViewComponent Cop { get; private set; }
    }
}
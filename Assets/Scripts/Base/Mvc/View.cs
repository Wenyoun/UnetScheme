using System.Collections.Generic;

namespace Base
{
    internal class View : IMView
    {
        private Dictionary<string, IMediator> mMediators;
        private Dictionary<string, List<IObserver>> mObservers;

        public View()
        {
            mMediators = new Dictionary<string, IMediator>();
            mObservers = new Dictionary<string, List<IObserver>>();
        }

        public void RegisterObserver(string name, IObserver observer)
        {
            List<IObserver> observers = null;
            if (!mObservers.TryGetValue(name, out observers))
            {
                observers = new List<IObserver>();
                mObservers.Add(name, observers);
            }
            observers.Add(observer);
        }

        public void RemoveObserver(string name, IObserver observer)
        {
            List<IObserver> observers = null;
            if (mObservers.TryGetValue(name, out observers))
            {
                for (int i = 0; i < observers.Count; i++)
                {
                    if (observers[i].Equals(observer))
                    {
                        observers.RemoveAt(i);
                        break;
                    }
                }
                if (observers.Count == 0)
                {
                    mObservers.Remove(name);
                }
            }
        }

        public void NotifyObservers(string name, object body)
        {
            List<IObserver> observers = null;
            if (mObservers.TryGetValue(name, out observers))
            {
                for (int i = 0; i < observers.Count; i++)
                {
                    observers[i].Notify(name, body);
                }
            }
        }

        public void RegisterMediator(IMediator mediator)
        {
            if (mMediators.ContainsKey(mediator.Name))
            {
                throw new System.ArgumentException("MediatorName=" + mediator.Name + " Already Register!");
            }
            mMediators.Add(mediator.Name, mediator);
            mediator.OnRegister();
            List<string> lists = mediator.Notifys();
            for (int i = 0; i < lists.Count; i++)
            {
                RegisterObserver(lists[i], mediator);
            }
        }

        public IMediator RetrieveMediator(string name)
        {
            IMediator mediator = null;
            mMediators.TryGetValue(name, out mediator);
            return mediator;
        }

        public IMediator RemoveMediator(string name)
        {
            IMediator mediator = null;
            if (mMediators.TryGetValue(name, out mediator))
            {
                List<string> lists = mediator.Notifys();
                for (int i = 0; i < lists.Count; i++)
                {
                    RemoveObserver(lists[i], mediator);
                }
                mMediators.Remove(name);
                mediator.OnRemove();
            }
            return mediator;
        }

        public bool HasMediator(string name)
        {
            return mMediators.ContainsKey(name);
        }
    }
}
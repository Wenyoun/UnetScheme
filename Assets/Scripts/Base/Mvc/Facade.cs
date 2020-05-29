namespace Base
{
    public class Facade
    {
        public static Facade Ins = new Facade();

        private IMView mView;
        private IModel mModel;

        public Facade()
        {
            mView = new View();
            mModel = new Model();
        }

        public void RegisterProxy(Proxy proxy)
        {
            mModel.RegisterProxy(proxy);
        }

        public T RetrieveProxy<T>() where T : Proxy
        {
            return mModel.RetrieveProxy<T>();
        }

        public Proxy RemoveProxy(System.Type type)
        {
            return mModel.RemoveProxy(type) as Proxy;
        }

        public bool HasProxy<T>() where T : Proxy
        {
            return mModel.HasProxy<T>();
        }

        public void RegisterMediator(Mediator mediator)
        {
            mView.RegisterMediator(mediator);
        }

        public Mediator RetrieveMediator(string name)
        {
            return (Mediator)mView.RetrieveMediator(name);
        }

        public Mediator RemoveMediator(string name)
        {
            return (Mediator)mView.RemoveMediator(name);
        }

        public bool HasMediator(string name)
        {
            return mView.HasMediator(name);
        }

        public void Notify(string name, object body = null)
        {
            mView.NotifyObservers(name, body);
        }
    }
}
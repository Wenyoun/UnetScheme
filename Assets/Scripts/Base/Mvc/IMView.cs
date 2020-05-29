namespace Base
{
    internal interface IMView
    {
        void RegisterObserver(string name, IObserver observer);

        void RemoveObserver(string name, IObserver observer);

        void NotifyObservers(string name, object body);

        void RegisterMediator(IMediator mediator);

        IMediator RetrieveMediator(string name);

        IMediator RemoveMediator(string name);

        bool HasMediator(string name);
    }
}
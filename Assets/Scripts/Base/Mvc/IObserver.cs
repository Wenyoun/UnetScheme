namespace Base
{
    internal interface IObserver
    {
        void Notify(string name, object body);
    }
}
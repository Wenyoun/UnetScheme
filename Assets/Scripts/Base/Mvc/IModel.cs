namespace Base
{
    internal interface IModel
    {
        void RegisterProxy( IProxy proxy );

        T RetrieveProxy<T>() where T : IProxy;

        IProxy RemoveProxy(System.Type type);

        bool HasProxy<T>() where T : IProxy;
    }
}
using System.Collections.Generic;

namespace Base
{
    internal interface IMediator : IObserver
    {
        string Name { get; }

        List<string> Notifys();

        void OnRegister();

        void OnRemove();
    }
}
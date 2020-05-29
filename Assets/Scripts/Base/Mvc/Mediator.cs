using System.Collections.Generic;

namespace Base
{
    public class Mediator : IMediator
    {
        public Mediator(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public virtual List<string> Notifys()
        {
            return new List<string>();
        }

        public virtual void OnRegister()
        {
        }

        public virtual void OnRemove()
        {
        }

        public virtual void Notify(string name, object body)
        {
        }
    }
}
using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public interface IProtocolHandler
    {
        void Register(NetworkConnection net);

        void Unregister(NetworkConnection net);
    }
}
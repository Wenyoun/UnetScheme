using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public interface IProtocolRegister
    {
        void Register(NetworkConnection net);

        void Unregister(NetworkConnection net);
    }
}
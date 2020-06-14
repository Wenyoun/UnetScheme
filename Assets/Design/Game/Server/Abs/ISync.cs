using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public interface ISync
    {
        bool IsSerialize();

        void Serialize(NetworkWriter writer);

        void Deserialize(NetworkReader reader);

    }

    public interface ISyncNotify
    {
        void SyncFinished();
    }
}
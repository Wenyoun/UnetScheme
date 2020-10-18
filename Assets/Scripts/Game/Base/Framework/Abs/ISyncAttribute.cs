using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public interface ISyncAttribute
    {
        uint SyncId { get; }

        bool IsSerialize();

        void Serialize(ByteBuffer writer);

        void Deserialize(ByteBuffer reader);
    }
}
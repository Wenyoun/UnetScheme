namespace Zyq.Game.Base
{
    public interface ISyncAttribute
    {
        long GetSyncId();
        
        bool IsDirty();

        void Serialize(ByteBuffer writer);

        void Deserialize(ByteBuffer reader);
    }
}
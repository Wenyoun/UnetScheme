using Nice.Game.Base;

[SyncClass]
public class BaseAttribute : ISyncAttribute
{
    [SyncField] public int Hp1;
    [SyncField] public int Hp2;

    public long GetSyncId()
    {
        return 2020812083018L;
    }

    public bool IsDirty()
    {
        return false;
    }

    public void Serialize(ByteBuffer writer)
    {
    }

    public void Deserialize(ByteBuffer reader)
    {
    }
}
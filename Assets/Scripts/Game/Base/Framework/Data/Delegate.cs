namespace Zyq.Game.Base
{
    public delegate void MsgDelegate(IBody body);

    public delegate void UpdateDelegate(float delta);

    public delegate void FixedUpdateDelegate(float delta);

    public delegate void LateUpdateDelegate();
}
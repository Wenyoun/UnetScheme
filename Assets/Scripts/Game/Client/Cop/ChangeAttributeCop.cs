using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class ChangeAttributeCop : AbsCop
    {
        protected override void Init()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            Entity.World.Messager.Register(MessageConstants.Sync_Attribute, (IBody body) =>
            {
                //UnityEngine.Debug.Log(attribute.Hp1 + "." + attribute.Hp11 + "," + attribute.Hp12);
            });
        }
    }
}
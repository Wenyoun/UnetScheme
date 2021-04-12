using Nice.Game.Base;

namespace Nice.Game.Client
{
    public class ChangeAttributeCop : AbsCop
    {
        protected override void Init()
        {
            BaseAttribute attribute = Entity.GetSyncAttribute<BaseAttribute>();
            Entity.World.RegisterMessage(MessageConstants.Sync_Attribute, (Body body) =>
            {
                //UnityEngine.Debug.Log(attribute.Hp1 + "." + attribute.Hp11 + "," + attribute.Hp12);
            });
        }
    }
}
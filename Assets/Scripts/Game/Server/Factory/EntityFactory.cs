using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public sealed class EntityFactory
    {
        private static uint id = 1;

        public static Entity CreatePlayer()
        {
            Entity entity = new Entity(id++, Group.Player);
            entity.OnInit();

            BaseAttribute attribute = entity.AddSyncAttribute(new BaseAttribute());
            attribute.Hp1 = 99999;
            attribute.Hp11 = "yinhuayong";

            entity.AddCop<ChangeAttributeCop>();

            return entity;
        }
    }
}
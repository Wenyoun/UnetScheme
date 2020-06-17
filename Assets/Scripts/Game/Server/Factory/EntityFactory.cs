using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public sealed class EntityFactory
    {
        private static uint id = 1;

        public static Entity CreatePlayer(Connection connection)
        {
            Entity entity = new Entity(id++, Group.Player);
            entity.OnInit();

            entity.AddSyncAttribute(new BaseAttribute());
            entity.AddFeture(new ConnectionFeture(connection));
            entity.AddCop<ChangeAttributeCop>();

            return entity;
        }
    }
}
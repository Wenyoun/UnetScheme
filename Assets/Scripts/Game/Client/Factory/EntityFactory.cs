using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public sealed class EntityFactory
    {
        public static Entity CreatePlayer(Connection connection, uint eid, uint gid)
        {
            Entity entity = new Entity(eid, gid);
            entity.OnInit();

            entity.AddSyncAttribute(new BaseAttribute());
            entity.AddFeture(new ConnectionFeture(connection));
            entity.AddCop<ChangeAttributeCop>();

            return entity;
        }
    }
}
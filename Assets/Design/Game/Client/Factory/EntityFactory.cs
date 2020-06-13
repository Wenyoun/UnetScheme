using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public sealed class EntityFactory
    {
        public static Entity CreatePlayer(uint eid, uint gid)
        {
            Entity entity = new Entity(eid, gid);
            entity.OnInit();
            return entity;
        }
    }
}
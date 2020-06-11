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

            entity.AddAttribute(new BaseAttribute(10));

            return entity;
        }
    }
}
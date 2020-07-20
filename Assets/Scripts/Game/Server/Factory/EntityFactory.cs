using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public sealed class EntityFactory
    {
        private static uint id = 1;

        public static Entity CreatePlayer(Connection connection, Vector3 position)
        {
            Entity entity = new Entity(id++, Group.Player);
            entity.OnInit();

            entity.AddSyncAttribute(new BaseAttribute());
            entity.AddFeture(new ConnectionFeture(connection));
            entity.AddFeture(new ServerObjectFeture(position));
            entity.AddCop<ServerChangeAttributeCop>();

            return entity;
        }
    }
}
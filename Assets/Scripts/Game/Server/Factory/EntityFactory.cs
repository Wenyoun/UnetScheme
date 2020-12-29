using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Server
{
    public sealed class EntityFactory
    {
        public static Entity CreatePlayer(Connection connection, Vector3 position)
        {
            Entity entity = new Entity();

            entity.AddSyncAttribute(new BaseAttribute());
            entity.AddFeture(new ConnectionFeture(connection));
            entity.AddFeture(new ServerObjectFeture(position));
            entity.AddCop<ServerChangeAttributeCop>();

            return entity;
        }
    }
}
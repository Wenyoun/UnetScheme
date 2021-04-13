using UnityEngine;
using Nice.Game.Base;

namespace Nice.Game.Client
{
    public sealed class EntityFactory
    {
        public static Entity CreatePlayer(Vector3 position)
        {
            Entity entity = new Entity();
            entity.OnInit();

            entity.AddSyncAttribute(new BaseAttribute());
            entity.AddCop<ChangeAttributeCop>();

            return entity;
        }
    }
}
using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public sealed class EntityFactory
    {
        public static Entity CreatePlayer(uint eid, Vector3 position)
        {
            Entity entity = new Entity(eid);
            entity.OnInit();

            entity.AddSyncAttribute(new BaseAttribute());
            entity.AddFeture(new ConnectionFeture(Client.Ins.Connection));
            entity.AddFeture(new ClientObjectFeture("Prefabs/Game/Tank", position));
            entity.AddCop<ChangeAttributeCop>();

            return entity;
        }
    }
}
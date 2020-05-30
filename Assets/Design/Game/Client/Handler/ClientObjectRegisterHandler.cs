using UnityEngine;
using Zyq.Game.Base;
using UnityEngine.Networking;

using Game;

namespace Zyq.Game.Client
{
    public class ClientObjectRegisterHandler
    {
        public static void Register()
        {
            ObjectEventRegister.Ins.Register(ObjectEventRegister.AddObject, (NetworkBehaviour behaviour) =>
            {
                //EntityMgr.AddEntity(EntityFactory.CreateTank(behaviour.netId.Value, Group.Tank, 100, ));
                Debug.Log("Client:11111111111111111111111111111");
            });

            ObjectEventRegister.Ins.Register(ObjectEventRegister.RemoveObject, (NetworkBehaviour behaviour) =>
            {
                Debug.Log("Client:222222222222222222222222222222");
            });
        }

        public static void Unregister()
        {
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.AddObject);
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.RemoveObject);
        }
    }
}
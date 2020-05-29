using UnityEngine;
using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class ServerObjectRegisterHandler
    {
        public static void Register()
        {
            ObjectEventRegister.Ins.Register(ObjectEventRegister.AddObject, (NetworkBehaviour behaviour) =>
            {
            });

            ObjectEventRegister.Ins.Register(ObjectEventRegister.RemoveObject, (NetworkBehaviour behaviour) =>
            {
            });
        }

        public static void Unregister()
        {
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.AddObject);
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.RemoveObject);
        }
    }
}
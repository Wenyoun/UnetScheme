using UnityEngine;
using Zyq.Game.Base;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class ObjectRegisterHandler
    {
        public static void Register()
        {
            ObjectEventRegister.Ins.Register(ObjectEventRegister.AddObject, (NetworkBehaviour behaviour) =>
            {
                Debug.Log("Server:1111111111111111111111111111");
            });

            ObjectEventRegister.Ins.Register(ObjectEventRegister.RemoveObject, (NetworkBehaviour behaviour) =>
            {
                Debug.Log("Server:1111111111111111111111111111");
            });
        }

        public static void Unregister()
        {
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.AddObject);
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.RemoveObject);
        }
    }
}
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
                Debug.Log("Server:21111111111111111111111111111");
            });

            ObjectEventRegister.Ins.Register(ObjectEventRegister.RemoveObject, (NetworkBehaviour behaviour) =>
            {
                Debug.Log("Server:22222222222222222222222222222");
            });
        }

        public static void Unregister()
        {
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.AddObject);
            ObjectEventRegister.Ins.Unregister(ObjectEventRegister.RemoveObject);
        }
    }
}
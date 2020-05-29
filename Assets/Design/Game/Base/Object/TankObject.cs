using UnityEngine.Networking;

namespace Zyq.Game.Base
{
    public class TankObject : NetworkBehaviour
    {
        private void Start()
        {
            ObjectEventRegister.Ins.Dispathcer(ObjectEventRegister.AddObject, this);
        }

        private void OnDestroy()
        {
            ObjectEventRegister.Ins.Dispathcer(ObjectEventRegister.RemoveObject, this);
        }
    }
}
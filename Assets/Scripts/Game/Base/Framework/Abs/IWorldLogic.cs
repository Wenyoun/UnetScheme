using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Game.Base
{
    public interface IWorldLogic
    {
        void OnInit(IWorld world);

        void OnRemove();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Game.Base
{
    public interface IWorldLogic
    {
        void Init(IWorld world);

        void Clear();
    }
}
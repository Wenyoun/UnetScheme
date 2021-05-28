using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;

namespace Nice.Game.Base
{
    public class SystemLoop
    {
        private static Action[] updates = new Action[0];

        [RuntimeInitializeOnLoadMethod]
        private static void OnInitLoadMethods()
        {
            AddSystemLoop<EarlyUpdate, SystemLoop>(OnUpdate);
        }

        private static void OnUpdate()
        {
            int length = updates.Length;
            if (length > 0)
            {
                for (int i = 0; i < updates.Length; ++i)
                {
                    updates[i].Invoke();
                }
            }
        }

        private static void AddSystemLoop<T1, T2>(PlayerLoopSystem.UpdateFunction update)
        {
            PlayerLoopSystem mainLoopSystem = PlayerLoop.GetDefaultPlayerLoop();
            if (FindLoopSystem<T1>(mainLoopSystem, out int index, out PlayerLoopSystem targetLoopSystem))
            {
                PlayerLoopSystem[] loopSystem = targetLoopSystem.subSystemList;
                int length = loopSystem.Length;
                Array.Resize(ref loopSystem, length + 1);
                loopSystem[length].type = typeof(T2);
                loopSystem[length].updateDelegate = update;
                targetLoopSystem.subSystemList = loopSystem;
                mainLoopSystem.subSystemList[index] = targetLoopSystem;
                PlayerLoop.SetPlayerLoop(mainLoopSystem);
            }
        }

        private static bool FindLoopSystem<T>(PlayerLoopSystem mainLoopSystem, out int index, out PlayerLoopSystem targetLoopSystem)
        {
            Type type = typeof(T);
            if (mainLoopSystem.subSystemList != null)
            {
                int length = mainLoopSystem.subSystemList.Length;
                for (int i = 0; i < length; ++i)
                {
                    PlayerLoopSystem subLoopSystem = mainLoopSystem.subSystemList[i];
                    if (type == subLoopSystem.type)
                    {
                        index = i;
                        targetLoopSystem = subLoopSystem;
                        return true;
                    }
                }
            }
            index = -1;
            targetLoopSystem = new PlayerLoopSystem();
            return false;
        }

        public static void AddUpdate(Action update)
        {
            int length = updates.Length;
            Array.Resize(ref updates, length + 1);
            updates[length] = update;
        }

        public static void RemoveUpdate(Action update)
        {
            int index = -1;
            List<Action> list = new List<Action>(updates);
            int length = list.Count;
            for (int i = 0; i < length; ++i)
            {
                if (list[i] == update)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                list.RemoveAt(index);
                updates = list.ToArray();
            }
        }
    }
}
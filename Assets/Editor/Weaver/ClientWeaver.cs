﻿using System.Collections.Generic;
using Mono.CecilX;
using UnityEngine.Networking;

namespace Zyq.Weaver
{
    public static class ClientWeaver
    {
        public static bool Weave(ModuleDefinition module)
        {
            TypeDefinition protocol = null;
            Dictionary<short, MethodDefinition> sendAttributeMethods;
            Dictionary<short, MethodDefinition> recvAttributeMethods;
            Dictionary<short, MethodDefinition> broadcastAttributeMethods;

            ParseAttribute.Parse(module, ref protocol, out sendAttributeMethods, out recvAttributeMethods, out broadcastAttributeMethods);

            if (sendAttributeMethods.Count > 0)
            {
                SendProcessor.Weave(module, sendAttributeMethods);
            }

            if (recvAttributeMethods.Count > 0)
            {
                RecvProcessor.Weave(module, recvAttributeMethods, protocol);
            }

            return sendAttributeMethods.Count > 0 || recvAttributeMethods.Count > 0;
        }
    }
}
﻿using Mono.CecilX;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class ServerWeaver
    {
        public static bool Weave(ModuleDefinition module)
        {
            TypeDefinition protocol = null;
            Dictionary<ushort, MethodData> sendAttributeMethods;
            Dictionary<ushort, MethodDefinition> recvAttributeMethods;
            Dictionary<ushort, MethodData> broadcastAttributeMethods;
            List<TypeDefinition> copTypes;
            List<TypeDefinition> syncAttributeTypes;
            Dictionary<FieldDefinition, MethodDefinition> gets;
            Dictionary<FieldDefinition, MethodDefinition> sets;

            ParseAttribute.Parse(module,
                                 ref protocol,
                                 out sendAttributeMethods,
                                 out recvAttributeMethods,
                                 out broadcastAttributeMethods,
                                 out copTypes,
                                 out syncAttributeTypes);

            if (sendAttributeMethods.Count > 0)
            {
                ServerSendProcessor.Weave(module, sendAttributeMethods);
            }

            if (recvAttributeMethods.Count > 0)
            {
                ServerRecvProcessor.Weave(module, recvAttributeMethods, protocol);
            }

            if (broadcastAttributeMethods.Count > 0)
            {
                ServerBroadcastProcessor.Weave(module, broadcastAttributeMethods);
            }

            /**
            if (copTypes.Count > 0)
            {
                ServerRecvCopProcessor.Weave(module, copTypes);
            }
            **/

            if (syncAttributeTypes.Count > 0)
            {
                ServerSyncProcessor.Weave(module, syncAttributeTypes, out gets, out sets);
                ServerReplaceProcessor.Weave(module, gets, sets);
            }

            ProfilerProcessor.Weave(module);

            return sendAttributeMethods.Count > 0 || recvAttributeMethods.Count > 0 || broadcastAttributeMethods.Count > 0 || copTypes.Count > 0 || syncAttributeTypes.Count > 0;
        }
    }
}
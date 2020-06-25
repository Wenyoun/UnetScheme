using System.Collections.Generic;
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
            List<TypeDefinition> copTypes;
            List<TypeDefinition> syncTypes;
            Dictionary<FieldDefinition, MethodDefinition> gets;
            Dictionary<FieldDefinition, MethodDefinition> sets;

            ParseAttribute.Parse(module,
                                 ref protocol,
                                 out sendAttributeMethods,
                                 out recvAttributeMethods,
                                 out broadcastAttributeMethods,
                                 out copTypes,
                                 out syncTypes);

            if (sendAttributeMethods.Count > 0)
            {
                ClientSendProcessor.Weave(module, sendAttributeMethods);
            }

            if (recvAttributeMethods.Count > 0)
            {
                RecvProcessor.Weave(module, recvAttributeMethods, protocol);
            }

            if (copTypes.Count > 0)
            {
                ClientRecvCopProcessor.Weave(module, copTypes);
            }

            if (syncTypes.Count > 0)
            {
                SyncProcessor.Weave(false, module, syncTypes, out gets, out sets);
            }

            return sendAttributeMethods.Count > 0 || recvAttributeMethods.Count > 0 || broadcastAttributeMethods.Count > 0 || syncTypes.Count > 0;
        }
    }
}
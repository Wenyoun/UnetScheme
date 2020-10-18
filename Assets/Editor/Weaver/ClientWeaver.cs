using Mono.CecilX;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ClientWeaver
    {
        public static bool Weave(ModuleDefinition module)
        {
            TypeDefinition protocol = null;
            Dictionary<ushort, MethodDefinition> sendAttributeMethods;
            Dictionary<ushort, MethodDefinition> recvAttributeMethods;
            Dictionary<ushort, MethodDefinition> broadcastAttributeMethods;
            List<TypeDefinition> copTypes;
            List<TypeDefinition> syncTypes;

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
                ClientRecvProcessor.Weave(module, recvAttributeMethods, protocol);
            }

            if (copTypes.Count > 0)
            {
                ClientRecvCopProcessor.Weave(module, copTypes);
            }

            if (syncTypes.Count > 0)
            {
                ClientSyncProcessor.Weave(module, syncTypes);
            }

            ProfilerProcessor.Weave(module);

            return sendAttributeMethods.Count > 0 || recvAttributeMethods.Count > 0 || copTypes.Count > 0 || syncTypes.Count > 0;
        }
    }
}
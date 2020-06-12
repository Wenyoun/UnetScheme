using System.Collections.Generic;
using Mono.CecilX;

namespace Zyq.Weaver {
    public class ServerWeaver {
        public static bool Weave(ModuleDefinition module) {
            TypeDefinition protocol = null;
            Dictionary<short, MethodDefinition> sendAttributeMethods;
            Dictionary<short, MethodDefinition> recvAttributeMethods;
            Dictionary<short, MethodDefinition> broadcastAttributeMethods = new Dictionary<short, MethodDefinition>();

            ParseAttribute.Parse(module, ref protocol, out sendAttributeMethods, out recvAttributeMethods, out broadcastAttributeMethods);

            if (sendAttributeMethods.Count > 0) {
                SendProcessor.Weave(module, sendAttributeMethods);
            }

            if (recvAttributeMethods.Count > 0) {
                RecvProcessor.Weave(module, recvAttributeMethods, protocol);
            }

            if (broadcastAttributeMethods.Count > 0) {
                BroadcastProcessor.Weave(module, broadcastAttributeMethods);
            }

            return sendAttributeMethods.Count > 0 || recvAttributeMethods.Count > 0;
        }
    }
}
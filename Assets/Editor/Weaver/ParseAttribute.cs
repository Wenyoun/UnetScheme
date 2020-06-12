using System.Collections.Generic;
using Mono.CecilX;

namespace Zyq.Weaver {
    public static class ParseAttribute {
        public static void Parse(ModuleDefinition module,
            ref TypeDefinition protocol,
            out Dictionary<short, MethodDefinition> send,
            out Dictionary<short, MethodDefinition> recv,
            out Dictionary<short, MethodDefinition> broadcast) {

            send = new Dictionary<short, MethodDefinition>();
            recv = new Dictionary<short, MethodDefinition>();
            broadcast = new Dictionary<short, MethodDefinition>();

            foreach (TypeDefinition type in module.Types) {
                foreach (MethodDefinition method in type.Methods) {
                    if (method.CustomAttributes.Count > 0) {
                        CustomAttribute methodAttr = method.CustomAttributes[0];
                        if (methodAttr.ConstructorArguments.Count > 0) {
                            short msgId = (short) methodAttr.ConstructorArguments[0].Value;
                            if (methodAttr.AttributeType.FullName == WeaverProgram.SendType.FullName) {
                                send.Add(msgId, method);
                            } else if (methodAttr.AttributeType.FullName == WeaverProgram.BroadcastType.FullName) {
                                broadcast.Add(msgId, method);
                            } else if (methodAttr.AttributeType.FullName == WeaverProgram.RecvType.FullName) {
                                recv.Add(msgId, method);
                            }
                        }
                    }
                }

                if (type.CustomAttributes.Count > 0) {
                    CustomAttribute typeAttr = type.CustomAttributes[0];
                    if (typeAttr.AttributeType.FullName == WeaverProgram.ProtocolType.FullName) {
                        protocol = type;
                    }
                }
            }
        }
    }
}
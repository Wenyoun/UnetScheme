using System.Collections.Generic;
using Mono.CecilX;

namespace Zyq.Weaver
{
    public static class ParseAttribute
    {
        public static void Parse(ModuleDefinition module,
                                ref TypeDefinition protocol,
                                out Dictionary<short, MethodDefinition> send,
                                out Dictionary<short, MethodDefinition> recv,
                                out Dictionary<short, MethodDefinition> broadcast,
                                out List<TypeDefinition> cops,
                                out List<TypeDefinition> syncs)
        {
            send = new Dictionary<short, MethodDefinition>();
            recv = new Dictionary<short, MethodDefinition>();
            broadcast = new Dictionary<short, MethodDefinition>();
            cops = new List<TypeDefinition>();
            syncs = new List<TypeDefinition>();

            foreach (TypeDefinition type in module.Types)
            {
                if (type.BaseType != null && type.BaseType.FullName == WeaverProgram.AbsCopType.FullName)
                {
                    cops.Add(type);
                }

                if (type.CustomAttributes.Count > 0 && type.CustomAttributes[0].AttributeType.FullName == WeaverProgram.SyncClassType.FullName)
                {
                    syncs.Add(type);
                }

                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.CustomAttributes.Count > 0)
                    {
                        CustomAttribute methodAttr = method.CustomAttributes[0];
                        if (methodAttr.ConstructorArguments.Count > 0)
                        {
                            short msgId = (short)methodAttr.ConstructorArguments[0].Value;
                            if (methodAttr.AttributeType.FullName == WeaverProgram.SendType.FullName)
                            {
                                send.Add(msgId, method);
                            }
                            else if (methodAttr.AttributeType.FullName == WeaverProgram.RecvType.FullName)
                            {
                                if (method.IsStatic)
                                {
                                    recv.Add(msgId, method);
                                }
                            }
                            else if (methodAttr.AttributeType.FullName == WeaverProgram.BroadcastType.FullName)
                            {
                                broadcast.Add(msgId, method);
                            }
                        }
                    }
                }

                if (type.CustomAttributes.Count > 0)
                {
                    CustomAttribute typeAttr = type.CustomAttributes[0];
                    if (typeAttr.AttributeType.FullName == WeaverProgram.ProtocolType.FullName)
                    {
                        protocol = type;
                    }
                }
            }
        }
    }
}
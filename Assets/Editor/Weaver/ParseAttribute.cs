using Mono.CecilX;
using UnityEngine;
using System.Collections.Generic;

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
                    continue;
                }

                if (type.CustomAttributes.Count > 0 && type.CustomAttributes[0].AttributeType.FullName == WeaverProgram.SyncClassType.FullName)
                {
                    syncs.Add(type);
                    continue;
                }

                if (type.CustomAttributes.Count > 0 && type.CustomAttributes[0].AttributeType.FullName == WeaverProgram.ProtocolType.FullName)
                {
                    if (protocol == null)
                    {
                        protocol = type;
                    }
                    else
                    {
                        Debug.LogError("只能存在一个用[" + type.CustomAttributes[0].AttributeType.FullName + "]修饰的class");
                    }
                    continue;
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
                                else
                                {
                                    Debug.LogError("[" + methodAttr.AttributeType.FullName + "]" + "只能修饰static方法,在" + type.FullName + "中的[" + method.FullName + "]方法");
                                }
                            }
                            else if (methodAttr.AttributeType.FullName == WeaverProgram.BroadcastType.FullName)
                            {
                                broadcast.Add(msgId, method);
                            }
                        }
                    }
                }
            }
        }
    }
}
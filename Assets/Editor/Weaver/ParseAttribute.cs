using Mono.CecilX;
using UnityEngine;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public struct MethodData {
        public byte Channel;
        public MethodDefinition MethodDef;

        public MethodData(byte channel, MethodDefinition methodDef) {
            Channel = channel;
            MethodDef = methodDef;
        }
    }
    
    public static class ParseAttribute
    {
        public static void Parse(ModuleDefinition module,
                                ref TypeDefinition protocol,
                                out Dictionary<ushort, MethodData> send,
                                out Dictionary<ushort, MethodDefinition> recv,
                                out Dictionary<ushort, MethodData> broadcast,
                                out List<TypeDefinition> cops,
                                out List<TypeDefinition> syncs)
        {
            send = new Dictionary<ushort, MethodData>();
            recv = new Dictionary<ushort, MethodDefinition>();
            broadcast = new Dictionary<ushort, MethodData>();
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
                            ushort msgId = (ushort)methodAttr.ConstructorArguments[0].Value;
                            if (methodAttr.AttributeType.FullName == WeaverProgram.SendType.FullName)
                            {
                                byte channel = (byte) methodAttr.ConstructorArguments[1].Value;
                                send.Add(msgId, new MethodData(channel, method));
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
                                byte channel = (byte) methodAttr.ConstructorArguments[1].Value;
                                broadcast.Add(msgId, new MethodData(channel, method));
                            }
                        }
                    }
                }
            }
        }
    }
}
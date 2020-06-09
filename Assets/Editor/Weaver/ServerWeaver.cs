using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Weaver {
    public class ServerWeaver {
        public static void Weave(ModuleDefinition module) {
            Dictionary<short, MethodDefinition> serverSendAttributeMethods = new Dictionary<short, MethodDefinition>();
            Dictionary<short, MethodDefinition> serverRecvAttributeMethods = new Dictionary<short, MethodDefinition>();

            TypeDefinition protocolType = null;

            foreach (TypeDefinition type in module.Types) {
                foreach (MethodDefinition method in type.Methods) {
                    if (method.CustomAttributes.Count > 0) {
                        CustomAttribute methodAttr = method.CustomAttributes[0];
                        if (methodAttr.ConstructorArguments.Count > 0) {
                            short msgId = (short) methodAttr.ConstructorArguments[0].Value;
                            if (methodAttr.AttributeType.FullName == WeaverProgram.SendType.FullName && methodAttr.ConstructorArguments.Count > 0) {
                                serverSendAttributeMethods.Add(msgId, method);
                            } else if (methodAttr.AttributeType.FullName == WeaverProgram.RecvType.FullName && methodAttr.ConstructorArguments.Count > 0) {
                                serverRecvAttributeMethods.Add(msgId, method);
                            }
                        }
                    }
                }

                if (type.CustomAttributes.Count > 0) {
                    CustomAttribute typeAttr = type.CustomAttributes[0];
                    if (typeAttr.AttributeType.FullName == WeaverProgram.ProtocolType.FullName) {
                        protocolType = type;
                    }
                }
            }

            if (protocolType != null) {
                MethodDefinition registerMethod = protocolType.Methods.Single(m => m.FullName.IndexOf("Register()") >= 0);
                ILProcessor registerProcessor = registerMethod.Body.GetILProcessor();
                Instruction firstRegisterInstruction = registerMethod.Body.Instructions[0];

                foreach (short key in serverRecvAttributeMethods.Keys) {
                    MethodDefinition serverMethod = serverRecvAttributeMethods[key];

                    MethodDefinition protoMethodImpl = new MethodDefinition("OnProtocol_" + key, MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
                    protocolType.Methods.Add(protoMethodImpl);
                    protoMethodImpl.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(NetworkMessage))));

                    ILProcessor pro = protoMethodImpl.Body.GetILProcessor();
                    pro.Emit(OpCodes.Nop);
                    pro.Emit(OpCodes.Ret);
                    Instruction first = protoMethodImpl.Body.Instructions[0];
                    pro.InsertBefore(first, pro.Create(OpCodes.Ldarg_1));
                    pro.InsertBefore(first, pro.Create(OpCodes.Ldfld, module.ImportReference(typeof(NetworkMessage).GetField("reader"))));

                    int index = 0;
                    protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(NetworkReader))));
                    foreach (ParameterDefinition pd in serverMethod.Parameters) {
                        if (index > 0) {
                            protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(pd.ParameterType)));
                            pro.InsertBefore(first, pro.Create(OpCodes.Stloc, index - 1));
                            pro.InsertBefore(first, pro.Create(OpCodes.Ldloc_0));
                            if (pd.ParameterType.FullName == "System.String") {
                                pro.InsertBefore(first, pro.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadString", Type.EmptyTypes))));
                            } else if (pd.ParameterType.FullName == "System.Int32") {
                                pro.InsertBefore(first, pro.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadInt32", Type.EmptyTypes))));
                            } else if (pd.ParameterType.FullName == "System.Single") {
                                pro.InsertBefore(first, pro.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadSingle", Type.EmptyTypes))));
                            } else if (pd.ParameterType.FullName == "System.Byte") {
                                pro.InsertBefore(first, pro.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadByte", Type.EmptyTypes))));
                            } else if (pd.ParameterType.FullName == "System.Int16") {
                                pro.InsertBefore(first, pro.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadInt16", Type.EmptyTypes))));
                            }
                        }
                        index++;
                    }

                    pro.InsertBefore(first, pro.Create(OpCodes.Stloc, index - 1));
                    pro.InsertBefore(first, pro.Create(OpCodes.Ldarg_0));
                    pro.InsertBefore(first, pro.Create(OpCodes.Call, module.ImportReference(protocolType.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0))));

                    index = 0;
                    foreach (ParameterDefinition pd in serverMethod.Parameters) {
                        if (index > 0) {
                            pro.InsertBefore(first, pro.Create(OpCodes.Ldloc, index));
                        }
                        index++;
                    }

                    pro.InsertBefore(first, pro.Create(OpCodes.Call, serverMethod));

                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Call, protocolType.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0)));
                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldc_I4, key));
                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldftn, protoMethodImpl));
                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                    registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("RegisterHandler", new Type[] { typeof(short), typeof(NetworkMessageDelegate) }))));
                }
            }

            MethodDefinition unregisterMethod = protocolType.Methods.Single(m => m.FullName.IndexOf("Unregister()") >= 0);
            ILProcessor unregisterProcessor = unregisterMethod.Body.GetILProcessor();
            Instruction firstUnregisterInstruction = unregisterMethod.Body.Instructions[0];

            foreach (short key in serverRecvAttributeMethods.Keys) {
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldarg_0));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, protocolType.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0)));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldc_I4, key));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("UnregisterHandler", new Type[] { typeof(short) }))));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Weaver {
    public static class RecvProcessor {
        public static void Weave(ModuleDefinition module, Dictionary<short, MethodDefinition> defs, TypeDefinition protocol) {
            if (module == null || defs.Count == 0 || protocol == null) {
                return;
            }

            MethodDefinition registerMethod = protocol.Methods.Single(m => m.FullName.IndexOf("Register()") >= 0);
            ILProcessor registerProcessor = registerMethod.Body.GetILProcessor();
            Instruction firstRegisterInstruction = registerMethod.Body.Instructions[0];

            foreach (short key in defs.Keys) {
                MethodDefinition method = defs[key];

                MethodDefinition protoMethodImpl = new MethodDefinition("OnProtocol_" + key, MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
                protocol.Methods.Add(protoMethodImpl);
                protoMethodImpl.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(NetworkMessage))));

                ILProcessor pro = protoMethodImpl.Body.GetILProcessor();
                pro.Emit(OpCodes.Nop);
                pro.Emit(OpCodes.Ret);
                Instruction first = protoMethodImpl.Body.Instructions[0];
                pro.InsertBefore(first, pro.Create(OpCodes.Ldarg_1));
                pro.InsertBefore(first, pro.Create(OpCodes.Ldfld, module.ImportReference(typeof(NetworkMessage).GetField("reader"))));

                int index = 0;
                protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(NetworkReader))));
                foreach (ParameterDefinition pd in method.Parameters) {
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
                pro.InsertBefore(first, pro.Create(OpCodes.Call, module.ImportReference(protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0))));

                index = 0;
                foreach (ParameterDefinition pd in method.Parameters) {
                    if (index > 0) {
                        pro.InsertBefore(first, pro.Create(OpCodes.Ldloc, index));
                    }
                    index++;
                }

                pro.InsertBefore(first, pro.Create(OpCodes.Call, method));

                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Call, protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0)));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldc_I4, key));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldftn, protoMethodImpl));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("RegisterHandler", new Type[] { typeof(short), typeof(NetworkMessageDelegate) }))));
            }

            MethodDefinition unregisterMethod = protocol.Methods.Single(m => m.FullName.IndexOf("Unregister()") >= 0);
            ILProcessor unregisterProcessor = unregisterMethod.Body.GetILProcessor();
            Instruction firstUnregisterInstruction = unregisterMethod.Body.Instructions[0];

            foreach (short key in defs.Keys) {
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldarg_0));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0)));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldc_I4, key));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("UnregisterHandler", new Type[] { typeof(short) }))));
            }
        }
    }
}
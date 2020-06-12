﻿using System;
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

                string name = "OnProtocol_" + key;

                MethodDefinition protoMethodImpl = null;
                foreach (MethodDefinition m in protocol.Resolve().Methods) {
                    if (m.Name == name) {
                        protoMethodImpl = m;
                        break;
                    }
                }

                if (protoMethodImpl == null) {
                    protoMethodImpl = new MethodDefinition(name, MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
                    protocol.Methods.Add(protoMethodImpl);
                }

                protoMethodImpl.Body.Instructions.Clear();
                protoMethodImpl.Parameters.Clear();
                protoMethodImpl.Body.Variables.Clear();
                protoMethodImpl.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(NetworkMessage))));

                ILProcessor processor = protoMethodImpl.Body.GetILProcessor();

                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Ldfld, module.ImportReference(typeof(NetworkMessage).GetField("reader"))));

                int index = 0;
                protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(NetworkReader))));
                foreach (ParameterDefinition pd in method.Parameters) {
                    if (index > 0) {
                        protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(pd.ParameterType)));
                        processor.Append(processor.Create(OpCodes.Stloc, index - 1));
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(InstructionFactory.CreateReadTypeInstruction(module, processor, pd.ParameterType.FullName));
                    }
                    index++;
                }

                processor.Append(processor.Create(OpCodes.Stloc, index - 1));
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Call, module.ImportReference(protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0))));

                index = 0;
                foreach (ParameterDefinition pd in method.Parameters) {
                    if (index > 0) {
                        processor.Append(processor.Create(OpCodes.Ldloc, index));
                    }
                    index++;
                }

                processor.Append(processor.Create(OpCodes.Call, method));
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Emit(OpCodes.Ret);

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
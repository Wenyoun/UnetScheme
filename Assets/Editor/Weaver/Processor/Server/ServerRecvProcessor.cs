using System;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using UnityEngine.Networking;

namespace Zyq.Weaver
{
    public static class ServerRecvProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<short, MethodDefinition> methods, TypeDefinition protocol)
        {
            if (module == null || methods.Count == 0 || protocol == null)
            {
                return;
            }

            MethodReference getConnection = ResolveHelper.ResolveMethod(protocol, "get_Connection");

            MethodDefinition registerMethod = ResolveHelper.ResolveMethod(protocol, "Register");
            registerMethod.Body.Variables.Clear();
            registerMethod.Body.Instructions.Clear();

            ILProcessor registerProcessor = registerMethod.Body.GetILProcessor();
            registerProcessor.Append(registerProcessor.Create(OpCodes.Nop));
            registerProcessor.Append(registerProcessor.Create(OpCodes.Ret));
            Instruction firstRegisterInstruction = registerMethod.Body.Instructions[0];

            foreach (short key in methods.Keys)
            {
                MethodDefinition method = methods[key];

                MethodDefinition protoMethodImpl = MethodFactory.CreateMethod(module, protocol, "OnProtocol_" + key, MethodAttributes.Private | MethodAttributes.HideBySig, true);
                protoMethodImpl.Parameters.Add(new ParameterDefinition(module.ImportReference(WeaverProgram.NetowrkMessageType)));
                protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.NetworkReaderType)));

                {
                    ILProcessor processor = protoMethodImpl.Body.GetILProcessor();
                    processor.Append(processor.Create(OpCodes.Nop));
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Ldfld, module.ImportReference(WeaverProgram.NetworkMessageReaderField)));

                    Collection<ParameterDefinition> parms = method.Parameters;
                    for (int i = 0; i < parms.Count; ++i)
                    {
                        if (i > 0)
                        {
                            ParameterDefinition pd = parms[i];
                            protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(pd.ParameterType)));
                            processor.Append(processor.Create(OpCodes.Stloc, i - 1));
                            processor.Append(processor.Create(OpCodes.Ldloc_0));
                            processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, pd.ParameterType.FullName));
                        }
                    }

                    processor.Append(processor.Create(OpCodes.Stloc, parms.Count - 1));
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Call, getConnection));
                    for (int i = 0; i < parms.Count; ++i)
                    {
                        if(i > 0)
                        {
                            processor.Append(processor.Create(OpCodes.Ldloc, i));
                        }
                    }
                    processor.Append(processor.Create(OpCodes.Call, method));
                    processor.Append(processor.Create(OpCodes.Nop));
                    processor.Append(processor.Create(OpCodes.Ret));
                }

                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Call, getConnection));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldc_I4, key));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldftn, protoMethodImpl));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ConnectionRegisterHandlerMethod)));
            }

            MethodDefinition unregisterMethod = ResolveHelper.ResolveMethod(protocol, "Unregister");
            unregisterMethod.Body.Variables.Clear();
            unregisterMethod.Body.Instructions.Clear();

            ILProcessor unregisterProcessor = unregisterMethod.Body.GetILProcessor();
            unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Nop));
            unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Ret));
            Instruction firstUnregisterInstruction = unregisterMethod.Body.Instructions[0];

            foreach (short key in methods.Keys)
            {
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldarg_0));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, getConnection));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldc_I4, key));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ConnectionUnregisterHandlerMethod)));
            }
        }
    }
}
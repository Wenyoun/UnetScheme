using System;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine.Networking;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ClientRecvProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<short, MethodDefinition> methods, TypeDefinition protocol)
        {
            if (module == null || methods.Count == 0 || protocol == null)
            {
                return;
            }

            MethodReference getConnection = ResolveHelper.ResolveMethod(protocol, "get_Connection");

            {
                //public void Register();
                MethodDefinition registerMethod = ResolveHelper.ResolveMethod(protocol, "Register");
                registerMethod.Body.Variables.Clear();
                registerMethod.Body.Instructions.Clear();

                ILProcessor registerProcessor = registerMethod.Body.GetILProcessor();
                registerProcessor.Append(registerProcessor.Create(OpCodes.Nop));

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
                        processor.Append(processor.Create(OpCodes.Stloc_0));

                        Collection<ParameterDefinition> parms = method.Parameters;
                        for (int i = 0; i < parms.Count; ++i)
                        {
                            ParameterDefinition parm = parms[i];
                            TypeDefinition parmType = parm.ParameterType.Resolve();
                            protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(parm.ParameterType)));

                            if (BaseTypeFactory.IsBaseType(parm.ParameterType.ToString()))
                            {
                                processor.Append(processor.Create(OpCodes.Ldloc_0));
                                processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, parm.ParameterType.FullName));
                                processor.Append(processor.Create(OpCodes.Stloc, i + 1));
                            }
                            else if (parmType != null && parmType.IsEnum)
                            {
                                processor.Append(processor.Create(OpCodes.Ldloc_0));
                                processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
                                processor.Append(processor.Create(OpCodes.Stloc, i + 1));
                            }
                            else if (parmType != null)
                            {
                                if(parmType.IsArray)
                                {
                                }
                                else if (parmType.IsValueType)
                                {
                                    MethodReference deserialize = StructMethodFactory.FindDeserialize(module, parmType);
                                    processor.Append(processor.Create(OpCodes.Ldloca, i + 1));
                                    processor.Append(processor.Create(OpCodes.Initobj, module.ImportReference(parmType)));
                                    processor.Append(processor.Create(OpCodes.Ldloca, i + 1));
                                    processor.Append(processor.Create(OpCodes.Ldloc_0));
                                    processor.Append(processor.Create(OpCodes.Call, deserialize));
                                }
                            }
                        }

                        for (int i = 0; i < parms.Count; ++i)
                        {
                            processor.Append(processor.Create(OpCodes.Ldloc, i + 1));
                        }

                        processor.Append(processor.Create(OpCodes.Call, method));
                        processor.Append(processor.Create(OpCodes.Nop));
                        processor.Append(processor.Create(OpCodes.Ret));
                    }

                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldarg_0));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Call, getConnection));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldc_I4, key));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldarg_0));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldftn, protoMethodImpl));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ConnectionRegisterHandlerMethod)));
                }
                registerProcessor.Append(registerProcessor.Create(OpCodes.Ret));
            }

            {
                //public void Unregister();
                MethodDefinition unregisterMethod = ResolveHelper.ResolveMethod(protocol, "Unregister");
                unregisterMethod.Body.Variables.Clear();
                unregisterMethod.Body.Instructions.Clear();

                ILProcessor unregisterProcessor = unregisterMethod.Body.GetILProcessor();
                unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Nop));
                foreach (short key in methods.Keys)
                {
                    unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Ldarg_0));
                    unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Callvirt, getConnection));
                    unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Ldc_I4, key));
                    unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ConnectionUnregisterHandlerMethod)));
                }
                unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Ret));
            }
        }
    }
}
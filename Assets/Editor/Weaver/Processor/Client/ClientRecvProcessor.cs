using System;
using Zyq.Game.Base;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ClientRecvProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<ushort, MethodDefinition> methods, TypeDefinition protocol)
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

                foreach (ushort msgId in methods.Keys)
                {
                    MethodDefinition method = methods[msgId];

                    if (!CheckHelper.CheckMethodParams(WeaverProgram.Client, method))
                    {
                        continue;
                    }

                    MethodDefinition protoMethodImpl = MethodFactory.CreateMethod(module, protocol, "OnProtocol_" + msgId, MethodAttributes.Private | MethodAttributes.HideBySig, true);
                    protoMethodImpl.Parameters.Add(new ParameterDefinition("msg", ParameterAttributes.None, module.ImportReference(WeaverProgram.ChannelMessageType)));
                    protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.ByteBufferType)));

                    {
                        ILProcessor processor = protoMethodImpl.Body.GetILProcessor();
                        processor.Append(processor.Create(OpCodes.Nop));
                        processor.Append(processor.Create(OpCodes.Ldarg_1));
                        processor.Append(processor.Create(OpCodes.Ldfld, module.ImportReference(WeaverProgram.ChannelMessageBufferField)));
                        processor.Append(processor.Create(OpCodes.Stloc_0));

                        List<int> indexs = new List<int>();
                        Collection<ParameterDefinition> parms = method.Parameters;
                        for (int i = 0; i < parms.Count; ++i)
                        {
                            ParameterDefinition parm = parms[i];
                            TypeDefinition parmType = parm.ParameterType.Resolve();
                            protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(parm.ParameterType)));
                            int index = protoMethodImpl.Body.Variables.Count - 1;
                            indexs.Add(index);

                            if (parm.ParameterType.IsArray)
                            {
                                ArrayReadFactory.CreateMethodVariableReadInstruction(module, protoMethodImpl, processor, parmType);
                            }
                            else if (BaseTypeFactory.IsBaseType(parmType))
                            {
                                processor.Append(processor.Create(OpCodes.Ldloc_0));
                                processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, parmType));
                                processor.Append(processor.Create(OpCodes.Stloc, index));
                            }
                            else if (parmType.IsValueType)
                            {
                                processor.Append(processor.Create(OpCodes.Ldloca, index));
                                processor.Append(processor.Create(OpCodes.Initobj, module.ImportReference(parmType)));
                                processor.Append(processor.Create(OpCodes.Ldloca, index));
                                processor.Append(processor.Create(OpCodes.Ldloc_0));
                                processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindDeserialize(module, parmType)));
                            }
                        }

                        for (int i = 0; i < indexs.Count; ++i)
                        {
                            processor.Append(processor.Create(OpCodes.Ldloc, indexs[i]));
                        }

                        processor.Append(processor.Create(OpCodes.Call, method));
                        processor.Append(processor.Create(OpCodes.Nop));
                        processor.Append(processor.Create(OpCodes.Ret));
                    }

                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldarg_0));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Call, getConnection));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldc_I4, msgId));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldarg_0));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Ldftn, protoMethodImpl));
                    registerProcessor.Append(registerProcessor.Create(OpCodes.Newobj, module.ImportReference(typeof(ChannelMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
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
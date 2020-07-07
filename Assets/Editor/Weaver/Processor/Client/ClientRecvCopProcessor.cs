using System;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using UnityEngine.Networking;

namespace Zyq.Weaver
{
    public static class ClientRecvCopProcessor
    {
        private struct MethodDetail
        {
            public short MsgId;
            public MethodDefinition Method;

            public MethodDetail(short msgId, MethodDefinition method)
            {
                MsgId = msgId;
                Method = method;
            }
        }

        public static void Weave(ModuleDefinition module, List<TypeDefinition> types)
        {
            if (module == null || types.Count == 0)
            {
                return;
            }

            List<MethodDetail> methods = new List<MethodDetail>();
            foreach (TypeDefinition type in types)
            {
                methods.Clear();

                foreach (MethodDefinition method in type.Methods)
                {
                    if (!method.IsStatic && method.CustomAttributes.Count > 0 && method.CustomAttributes[0].AttributeType.FullName == WeaverProgram.RecvType.FullName)
                    {
                        short msgId = (short)method.CustomAttributes[0].ConstructorArguments[0].Value;
                        methods.Add(new MethodDetail(msgId, method));
                    }
                }

                if (methods.Count > 0)
                {
                    MethodDefinition onInitMethod = ResolveHelper.ResolveMethod(type, "OnInit");
                    MethodDefinition onRemoveMethod = ResolveHelper.ResolveMethod(type, "OnRemove");

                    if (onRemoveMethod == null)
                    {
                        onRemoveMethod = MethodFactory.CreateMethod(module, type, "OnRemove", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual);
                        ILProcessor pro = onRemoveMethod.Body.GetILProcessor();
                        Instruction ins = onRemoveMethod.Body.Instructions[0];
                        pro.InsertAfter(ins, pro.Create(OpCodes.Call, module.ImportReference(WeaverProgram.AbsCopOnRemoveMethod)));
                        pro.InsertAfter(ins, pro.Create(OpCodes.Ldarg_0));
                    }

                    string onRegisterHandlerName = "OnRegister" + type.Name + "Handler";
                    MethodDefinition registerMethod = MethodFactory.CreateMethod(module, type, onRegisterHandlerName, MethodAttributes.Private | MethodAttributes.HideBySig);
                    {
                        registerMethod.Body.Instructions.Clear();
                        registerMethod.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.ConnectionFetureType)));
                        registerMethod.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));
                        {
                            ILProcessor processor = registerMethod.Body.GetILProcessor();
                            Instruction ret = processor.Create(OpCodes.Ret);
                            processor.Append(processor.Create(OpCodes.Nop));
                            processor.Append(processor.Create(OpCodes.Ldarg_0));
                            processor.Append(processor.Create(OpCodes.Call, module.ImportReference(WeaverProgram.AbsCopGetEntityMethod)));
                            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.IEntityGetFetureMethod.MakeGenericMethod(WeaverProgram.ConnectionFetureType))));
                            processor.Append(processor.Create(OpCodes.Stloc_0));
                            processor.Append(processor.Create(OpCodes.Ldloc_0));
                            processor.Append(processor.Create(OpCodes.Ldnull));
                            processor.Append(processor.Create(OpCodes.Cgt_Un));
                            processor.Append(processor.Create(OpCodes.Stloc_1));
                            processor.Append(processor.Create(OpCodes.Ldloc_1));
                            processor.Append(processor.Create(OpCodes.Brfalse, ret));
                            foreach (MethodDetail wrapper in methods)
                            {
                                MethodDefinition handler = MethodFactory.CreateMethod(module, type, "OnHandlerProtocol_" + wrapper.MsgId, MethodAttributes.Private | MethodAttributes.HideBySig, true);
                                {
                                    handler.Parameters.Add(new ParameterDefinition(module.ImportReference(WeaverProgram.NetowrkMessageType)));
                                    handler.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.NetworkReaderType)));

                                    ILProcessor handlerProcessor = handler.Body.GetILProcessor();
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Nop));
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldarg_1));
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldfld, module.ImportReference(WeaverProgram.NetworkMessageReaderField)));
                                    Collection<ParameterDefinition> parms = wrapper.Method.Parameters;
                                    for (int i = 0; i < parms.Count; ++i)
                                    {
                                        ParameterDefinition pd = parms[i];
                                        handler.Body.Variables.Add(new VariableDefinition(module.ImportReference(pd.ParameterType)));
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Stloc, i));
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloc_0));
                                        handlerProcessor.Append(BaseTypeFactory.CreateReadTypeInstruction(module, processor, pd.ParameterType.FullName));
                                    }
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Stloc, parms.Count));
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldarg_0));
                                    for (int i = 0; i < parms.Count; ++i)
                                    {
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloc, i + 1));
                                    }
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Call, wrapper.Method));
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Nop));
                                    handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ret));
                                }

                                {
                                    processor.Append(processor.Create(OpCodes.Nop));
                                    processor.Append(processor.Create(OpCodes.Ldloc_0));
                                    processor.Append(processor.Create(OpCodes.Ldc_I4, wrapper.MsgId));
                                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                                    processor.Append(processor.Create(OpCodes.Ldftn, handler));
                                    processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                                    processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ConnectionFetureRegisterHandlerMethod)));
                                    processor.Append(processor.Create(OpCodes.Nop));
                                }
                            }
                            processor.Append(ret);
                        }

                        {
                            ILProcessor init = onInitMethod.Body.GetILProcessor();
                            Instruction ins1 = onInitMethod.Body.Instructions[onInitMethod.Body.Instructions.Count - 1];
                            init.InsertBefore(ins1, init.Create(OpCodes.Ldarg_0));
                            init.InsertBefore(ins1, init.Create(OpCodes.Call, registerMethod));
                            init.InsertBefore(ins1, init.Create(OpCodes.Nop));
                        }
                    }

                    string onUnregisterHandlerName = "OnUnregister" + type.Name + "Handler";
                    MethodDefinition unregisterMethod = MethodFactory.CreateMethod(module, type, onUnregisterHandlerName, MethodAttributes.Private | MethodAttributes.HideBySig);
                    {
                        unregisterMethod.Body.Instructions.Clear();
                        unregisterMethod.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.ConnectionFetureType)));
                        unregisterMethod.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));
                        {
                            ILProcessor processor = unregisterMethod.Body.GetILProcessor();
                            Instruction ret = processor.Create(OpCodes.Ret);
                            processor.Append(processor.Create(OpCodes.Nop));
                            processor.Append(processor.Create(OpCodes.Ldarg_0));
                            processor.Append(processor.Create(OpCodes.Call, module.ImportReference(WeaverProgram.AbsCopGetEntityMethod)));
                            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.IEntityGetFetureMethod.MakeGenericMethod(WeaverProgram.ConnectionFetureType))));
                            processor.Append(processor.Create(OpCodes.Stloc_0));
                            processor.Append(processor.Create(OpCodes.Ldloc_0));
                            processor.Append(processor.Create(OpCodes.Ldnull));
                            processor.Append(processor.Create(OpCodes.Cgt_Un));
                            processor.Append(processor.Create(OpCodes.Stloc_1));
                            processor.Append(processor.Create(OpCodes.Ldloc_1));
                            processor.Append(processor.Create(OpCodes.Brfalse, ret));
                            foreach (MethodDetail wrapper in methods)
                            {
                                processor.Append(processor.Create(OpCodes.Nop));
                                processor.Append(processor.Create(OpCodes.Ldloc_0));
                                processor.Append(processor.Create(OpCodes.Ldc_I4, wrapper.MsgId));
                                processor.Append(processor.Create(OpCodes.Call, module.ImportReference(WeaverProgram.ConnectionFetureUnregisterHandlerMethod)));
                            }
                            processor.Append(ret);
                        }

                        {
                            ILProcessor remo = onRemoveMethod.Body.GetILProcessor();
                            Instruction ins2 = onRemoveMethod.Body.Instructions[onRemoveMethod.Body.Instructions.Count - 1];
                            remo.InsertBefore(ins2, remo.Create(OpCodes.Ldarg_0));
                            remo.InsertBefore(ins2, remo.Create(OpCodes.Call, unregisterMethod));
                            remo.InsertBefore(ins2, remo.Create(OpCodes.Nop));
                        }
                    }
                }
            }
        }
    }
}
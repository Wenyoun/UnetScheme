using System;
using Zyq.Game.Base;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ServerRecvCopProcessor
    {
        private struct MethodDetail
        {
            public ushort MsgId;
            public MethodDefinition Method;

            public MethodDetail(ushort msgId, MethodDefinition method)
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
                    if (!method.IsStatic &&
                        method.CustomAttributes.Count > 0 &&
                        method.CustomAttributes[0].AttributeType.FullName == WeaverProgram.RecvType.FullName)
                    {
                        ushort msgId = (ushort)method.CustomAttributes[0].ConstructorArguments[0].Value;
                        methods.Add(new MethodDetail(msgId, method));
                    }
                }

                if (methods.Count > 0)
                {
                    MethodDefinition onInitMethod = ResolveHelper.ResolveMethod(type, "OnInit");
                    MethodDefinition onRemoveMethod = ResolveHelper.ResolveMethod(type, "OnRemove");

                    if (onRemoveMethod == null)
                    {
                        onRemoveMethod = MethodFactory.CreateMethod(module,
                                                                    type,
                                                                    "OnRemove",
                                                                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual);

                        ILProcessor pro = onRemoveMethod.Body.GetILProcessor();
                        Instruction ins = onRemoveMethod.Body.Instructions[onRemoveMethod.Body.Instructions.Count - 1];
                        pro.InsertBefore(ins, pro.Create(OpCodes.Ldarg_0));
                        pro.InsertBefore(ins, pro.Create(OpCodes.Call, module.ImportReference(WeaverProgram.AbsCopOnRemoveMethod)));
                    }

                    {
                        string registerHandlerName = "OnRegister" + type.Name + "Handler";
                        bool isExistsRegisterMethod = ResolveHelper.HasMethod(type, registerHandlerName);
                        MethodDefinition registerMethod = MethodFactory.CreateMethod(module,
                                                                                     type,
                                                                                     registerHandlerName,
                                                                                     MethodAttributes.Private | MethodAttributes.HideBySig,
                                                                                     true);
                        {
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
                                    if (!CheckHelper.CheckMethodParams(WeaverProgram.Client, wrapper.Method))
                                    {
                                        continue;
                                    }

                                    MethodDefinition handlerMethodImpl = MethodFactory.CreateMethod(module,
                                                                                          type,
                                                                                          "OnHandlerProtocol_" + wrapper.MsgId,
                                                                                          MethodAttributes.Private | MethodAttributes.HideBySig,
                                                                                          true);
                                    {
                                        handlerMethodImpl.Parameters.Add(new ParameterDefinition("msg", ParameterAttributes.None, module.ImportReference(WeaverProgram.ChannelMessageType)));
                                        handlerMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.ByteBufferType)));

                                        ILProcessor handlerProcessor = handlerMethodImpl.Body.GetILProcessor();
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Nop));
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldarg_1));
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldfld, module.ImportReference(WeaverProgram.ChannelMessageBufferField)));
                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Stloc_0));

                                        List<int> indexs = new List<int>();
                                        Collection<ParameterDefinition> parms = wrapper.Method.Parameters;
                                        for (int i = 0; i < parms.Count; ++i)
                                        {
                                            ParameterDefinition parm = parms[i];
                                            TypeDefinition parmType = parm.ParameterType.Resolve();
                                            handlerMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(parm.ParameterType)));

                                            int index = handlerMethodImpl.Body.Variables.Count - 1;
                                            indexs.Add(index);

                                            if (parm.ParameterType.IsArray)
                                            {
                                                ArrayReadFactory.CreateMethodVariableReadInstruction(module, handlerMethodImpl, handlerProcessor, parmType);
                                            }
                                            else if (BaseTypeFactory.IsBaseType(parmType))
                                            {
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloc_0));
                                                handlerProcessor.Append(BaseTypeFactory.CreateReadInstruction(module, handlerProcessor, parmType));
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Stloc, index));
                                            }
                                            else if (parmType.IsValueType)
                                            {
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloca, index));
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Initobj, module.ImportReference(parmType)));
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloca, index));
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloc_0));
                                                handlerProcessor.Append(handlerProcessor.Create(OpCodes.Call, StructMethodFactory.FindDeserialize(module, parmType)));
                                            }
                                        }

                                        handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldarg_0));

                                        for (int i = 0; i < indexs.Count; ++i)
                                        {
                                            handlerProcessor.Append(handlerProcessor.Create(OpCodes.Ldloc, indexs[i]));
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
                                        processor.Append(processor.Create(OpCodes.Ldftn, handlerMethodImpl));
                                        processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(typeof(ChannelMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                                        processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ConnectionFetureRegisterHandlerMethod)));
                                        processor.Append(processor.Create(OpCodes.Nop));
                                    }
                                }
                                processor.Append(ret);
                            }

                            if (!isExistsRegisterMethod)
                            {
                                ILProcessor pro = onInitMethod.Body.GetILProcessor();
                                Instruction ins = onInitMethod.Body.Instructions[onInitMethod.Body.Instructions.Count - 1];
                                pro.InsertBefore(ins, pro.Create(OpCodes.Ldarg_0));
                                pro.InsertBefore(ins, pro.Create(OpCodes.Call, registerMethod));
                                pro.InsertBefore(ins, pro.Create(OpCodes.Nop));
                            }
                        }
                    }

                    {
                        string unregisterHandlerName = "OnUnRegister" + type.Name + "Handler";
                        bool isExistsUnregisterMethod = ResolveHelper.HasMethod(type, unregisterHandlerName);
                        MethodDefinition unregisterMethod = MethodFactory.CreateMethod(module,
                                                                                       type,
                                                                                       unregisterHandlerName,
                                                                                       MethodAttributes.Private | MethodAttributes.HideBySig,
                                                                                       true);
                        {
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

                            if (!isExistsUnregisterMethod)
                            {
                                ILProcessor pro = onRemoveMethod.Body.GetILProcessor();
                                Instruction ins = onRemoveMethod.Body.Instructions[onRemoveMethod.Body.Instructions.Count - 1];
                                pro.InsertBefore(ins, pro.Create(OpCodes.Ldarg_0));
                                pro.InsertBefore(ins, pro.Create(OpCodes.Call, unregisterMethod));
                                pro.InsertBefore(ins, pro.Create(OpCodes.Nop));
                            }
                        }
                    }
                }
            }
        }
    }
}
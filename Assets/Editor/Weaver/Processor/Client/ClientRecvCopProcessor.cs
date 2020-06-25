using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Weaver
{
    public static class ClientRecvCopProcessor
    {
        private struct Wrapper
        {
            public short MsgId;
            MethodDefinition Method;

            public Wrapper(short msgId, MethodDefinition method)
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

            List<Wrapper> methods = new List<Wrapper>();
            foreach (TypeDefinition type in types)
            {
                methods.Clear();

                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.CustomAttributes.Count > 0 && method.CustomAttributes[0].AttributeType.FullName == WeaverProgram.RecvType.FullName)
                    {
                        short msgId = (short)method.CustomAttributes[0].ConstructorArguments[0].Value;
                        methods.Add(new Wrapper(msgId, method));
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

                        foreach (Wrapper wrapper in methods)
                        {
                            MethodDefinition handler = MethodFactory.CreateMethod(module, type, "OnProtocol_" + wrapper.MsgId, MethodAttributes.Private | MethodAttributes.HideBySig);
                            handler.Parameters.Add(new ParameterDefinition(module.ImportReference(WeaverProgram.NetowrkMessageType)));

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
                            /**
                            processor.Append(processor.Create(OpCodes.Nop));
                            processor.Append(processor.Create(OpCodes.Ldloc_0));
                            processor.Append(processor.Create(OpCodes.Ldc_I4, wrapper.MsgId));
                            processor.Append(processor.Create(OpCodes.Ldarg_0));
                            **/
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
                        ILProcessor remo = onRemoveMethod.Body.GetILProcessor();
                        Instruction ins2 = onRemoveMethod.Body.Instructions[onRemoveMethod.Body.Instructions.Count - 1];
                        remo.InsertBefore(ins2, remo.Create(OpCodes.Ldarg_0));
                        remo.InsertBefore(ins2, remo.Create(OpCodes.Call, unregisterMethod));
                        remo.InsertBefore(ins2, remo.Create(OpCodes.Nop));
                    }
                }
            }

            /**
            MethodDefinition registerMethod = protocol.Methods.Single(m => m.FullName.IndexOf("Register()") >= 0);
            registerMethod.Body.Variables.Clear();
            registerMethod.Body.Instructions.Clear();

            ILProcessor registerProcessor = registerMethod.Body.GetILProcessor();
            registerProcessor.Append(registerProcessor.Create(OpCodes.Nop));
            registerProcessor.Append(registerProcessor.Create(OpCodes.Ret));
            Instruction firstRegisterInstruction = registerMethod.Body.Instructions[0];

            foreach (short key in defs.Keys)
            {
                MethodDefinition method = defs[key];

                string name = "OnProtocol_" + key;

                MethodDefinition protoMethodImpl = null;
                foreach (MethodDefinition m in protocol.Methods)
                {
                    if (m.Name == name)
                    {
                        protoMethodImpl = m;
                        break;
                    }
                }

                if (protoMethodImpl == null)
                {
                    protoMethodImpl = new MethodDefinition(name, MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
                    protocol.Methods.Add(protoMethodImpl);
                    protoMethodImpl.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(NetworkMessage))));
                }

                protoMethodImpl.Body.Variables.Clear();
                protoMethodImpl.Body.Instructions.Clear();

                ILProcessor processor = protoMethodImpl.Body.GetILProcessor();
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ret));

                Instruction first = protoMethodImpl.Body.Instructions[0];
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_1));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldfld, module.ImportReference(typeof(NetworkMessage).GetField("reader"))));

                int index = 0;
                protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(NetworkReader))));
                foreach (ParameterDefinition pd in method.Parameters)
                {
                    if (index > 0)
                    {
                        protoMethodImpl.Body.Variables.Add(new VariableDefinition(module.ImportReference(pd.ParameterType)));
                        processor.InsertBefore(first, processor.Create(OpCodes.Stloc, index - 1));
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                        processor.InsertBefore(first, InstructionFactory.CreateReadTypeInstruction(module, processor, pd.ParameterType.FullName));
                    }
                    index++;
                }

                processor.InsertBefore(first, processor.Create(OpCodes.Stloc, index - 1));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Call, module.ImportReference(protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0))));

                index = 0;
                foreach (ParameterDefinition pd in method.Parameters)
                {
                    if (index > 0)
                    {
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, index));
                    }
                    index++;
                }

                processor.InsertBefore(first, processor.Create(OpCodes.Call, method));

                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Call, protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0)));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldc_I4, key));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldarg_0));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Ldftn, protoMethodImpl));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkMessageDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));
                registerProcessor.InsertBefore(firstRegisterInstruction, registerProcessor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("RegisterHandler", new Type[] { typeof(short), typeof(NetworkMessageDelegate) }))));
            }

            MethodDefinition unregisterMethod = protocol.Methods.Single(m => m.FullName.IndexOf("Unregister()") >= 0);
            unregisterMethod.Body.Variables.Clear();
            unregisterMethod.Body.Instructions.Clear();

            ILProcessor unregisterProcessor = unregisterMethod.Body.GetILProcessor();
            unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Nop));
            unregisterProcessor.Append(unregisterProcessor.Create(OpCodes.Ret));
            Instruction firstUnregisterInstruction = unregisterMethod.Body.Instructions[0];

            foreach (short key in defs.Keys)
            {
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldarg_0));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, protocol.Methods.Single(m => m.FullName.IndexOf("get_Connection") >= 0)));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Ldc_I4, key));
                unregisterProcessor.InsertBefore(firstUnregisterInstruction, unregisterProcessor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("UnregisterHandler", new Type[] { typeof(short) }))));
            }
        **/
        }
    }
}
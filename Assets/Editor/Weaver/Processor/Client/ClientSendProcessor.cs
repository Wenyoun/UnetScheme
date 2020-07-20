﻿using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ClientSendProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<short, MethodDefinition> methods)
        {
            foreach (short key in methods.Keys)
            {
                MethodDefinition method = methods[key];

                ILProcessor processor = method.Body.GetILProcessor();
                method.Body.Variables.Clear();
                method.Body.Instructions.Clear();

                method.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.NetworkWriterType)));

                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(WeaverProgram.NetworkWriterCtorMethod)));
                processor.Append(processor.Create(OpCodes.Stloc_0));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Ldc_I4, key));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.NetworkWriterStartMessageMethod)));

                Collection<ParameterDefinition> parms = method.Parameters;
                for (int i = 0; i < parms.Count; ++i)
                {
                    ParameterDefinition parm = parms[i];
                    byte index = (byte)(method.IsStatic ? i : i + 1);
                    TypeDefinition parmType = parm.ParameterType.Resolve();

                    if (parm.ParameterType.IsArray)
                    {
                        ArrayWriteFactory.CreateMethodParamWriteInstruction(module, method, processor, parmType, index);
                    }
                    else if (BaseTypeFactory.IsBaseType(parmType.ToString()) || parmType.IsEnum)
                    {
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Ldarg_S, index));
                        if (parmType.IsEnum)
                        {
                            processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));
                        }
                        else
                        {
                            processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, parmType.ToString()));
                        }
                    }
                    else if (parmType.IsValueType)
                    {
                        MethodReference serialize = StructMethodFactory.FindSerialize(module, parmType);
                        processor.Append(processor.Create(OpCodes.Ldarga_S, index));
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Call, serialize));
                    }
                }

                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.NetworkWriterFinishMessageMethod)));
                processor.Append(processor.Create(OpCodes.Ldsfld, module.ImportReference(WeaverProgram.ClientInsField)));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ClientSendMethod)));
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ret));
            }
        }
    }
}
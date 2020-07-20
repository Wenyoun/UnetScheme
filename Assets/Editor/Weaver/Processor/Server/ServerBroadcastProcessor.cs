using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ServerBroadcastProcessor
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
                    TypeDefinition parmType = parm.ParameterType.Resolve();
                    byte index = (byte)(method.IsStatic ? i : i + 1);

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
                        processor.Append(processor.Create(OpCodes.Ldarga_S, index));
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindSerialize(module, parmType)));
                    }
                }

                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.NetworkWriterFinishMessageMethod)));
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ldsfld, module.ImportReference(WeaverProgram.ServerInsField)));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ServerBroadcastMethod)));
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ret));
            }
        }
    }
}
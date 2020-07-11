using Mono.CecilX;
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
                    TypeDefinition parmType = parm.ParameterType as TypeDefinition;

                    if (BaseTypeFactory.IsBaseType(parm.ParameterType.ToString()))
                    {
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Ldarg_S, index));
                        processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, parm.ParameterType.ToString()));
                    }
                    else if(parmType != null && parmType.IsEnum)
                    {
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Ldarg_S, index));
                        processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));
                    }
                    else if (parmType != null)
                    {
                        if (parmType.IsArray)
                        {
                        }
                        else
                        {
                            if (parmType != null && parmType.IsValueType)
                            {
                                MethodDefinition serialize = StructMethodFactory.CreateSerialize(module, parmType);
                                processor.Append(processor.Create(OpCodes.Ldarga_S, index));
                                processor.Append(processor.Create(OpCodes.Ldloc_0));
                                processor.Append(processor.Create(OpCodes.Call, serialize));
                            }
                        }
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
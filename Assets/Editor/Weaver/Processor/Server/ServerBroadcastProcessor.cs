using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;

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
                    ParameterDefinition pd = parms[i];
                    processor.Append(processor.Create(OpCodes.Nop));
                    processor.Append(processor.Create(OpCodes.Ldloc_0));
                    processor.Append(processor.Create(OpCodes.Ldarg_S, (byte)(method.IsStatic ? i : i + 1)));
                    processor.Append(BaseTypeFactory.CreateWriteTypeInstruction(module, processor, pd.ParameterType.ToString()));
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
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;

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
                processor.Append(processor.Create(OpCodes.Ret));

                Instruction first = method.Body.Instructions[0];

                processor.InsertBefore(first, processor.Create(OpCodes.Nop));
                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(WeaverProgram.NetworkWriterCtorMethod)));
                processor.InsertBefore(first, processor.Create(OpCodes.Stloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, key));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.NetworkWriterStartMessageMethod)));

                Collection<ParameterDefinition> parms = method.Parameters;
                for (int i = 0; i < parms.Count; ++i)
                {
                    if (i > 0)
                    {
                        processor.InsertBefore(first, processor.Create(OpCodes.Nop));
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_S, (byte)(method.IsStatic ? i : i + 1)));
                        processor.InsertBefore(first, InstructionFactory.CreateWriteTypeInstruction(module, processor, parms[i].ParameterType.ToString()));
                    }
                }

                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.NetworkWriterFinishMessageMethod)));
                processor.InsertBefore(first, processor.Create(OpCodes.Nop));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldsfld, module.ImportReference(WeaverProgram.ClientInsField)));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ClientSendMethod)));
            }
        }
    }
}
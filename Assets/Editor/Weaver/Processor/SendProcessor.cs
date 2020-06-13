using System;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Weaver
{
    public static class SendProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<short, MethodDefinition> defs)
        {
            foreach (short key in defs.Keys)
            {
                MethodDefinition method = defs[key];
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
                int index = 0;
                foreach (ParameterDefinition pd in method.Parameters)
                {
                    if (index > 0)
                    {
                        processor.InsertBefore(first, processor.Create(OpCodes.Nop));
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_S, pd));
                        processor.InsertBefore(first, InstructionFactory.CreateWriteTypeInstruction(module, processor, pd.ParameterType.ToString()));
                    }
                    index++;
                }
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.NetworkWriterFinishMessageMethod)));

                processor.InsertBefore(first, processor.Create(OpCodes.Nop));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("Send", new Type[] { typeof(NetworkWriter) }))));
            }
        }
    }
}
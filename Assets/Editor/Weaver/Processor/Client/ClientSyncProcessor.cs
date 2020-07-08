using Mono.CecilX;
using Mono.CecilX.Cil;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ClientSyncProcessor
    {
        public static void Weave(ModuleDefinition module, List<TypeDefinition> syncs)
        {
            foreach (TypeDefinition type in syncs)
            {
                List<FieldDefinition> fields = new List<FieldDefinition>();

                foreach (FieldDefinition field in type.Fields)
                {
                    if (field.CustomAttributes.Count > 0 && field.CustomAttributes[0].AttributeType.FullName == WeaverProgram.SyncFieldType.FullName)
                    {
                        fields.Add(field);
                    }
                }

                if (fields.Count == 0)
                {
                    return;
                }

                //Deserialize
                MethodDefinition deserialize = ResolveHelper.ResolveMethod(type, "Deserialize");

                ILProcessor processor = deserialize.Body.GetILProcessor();
                processor.Body.Instructions.Clear();

                Instruction ret = processor.Create(OpCodes.Ret);
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, module.ImportReference(typeof(long)).ToString()));
                processor.Append(processor.Create(OpCodes.Stloc_0));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Ldc_I8, 0L));
                processor.Append(processor.Create(OpCodes.Cgt));
                processor.Append(processor.Create(OpCodes.Stloc_1));
                processor.Append(processor.Create(OpCodes.Ldloc_1));
                processor.Append(processor.Create(OpCodes.Brfalse, ret));

                deserialize.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(long))));
                deserialize.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

                int offset = deserialize.Body.Variables.Count;

                for (int i = 0; i < fields.Count; ++i)
                {
                    WriteDeserializeInstruction(module, deserialize, processor, fields[i], i, offset);
                }

                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(ret);
            }
        }

        private static void WriteDeserializeInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, FieldDefinition field, int index, int offset)
        {
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            Instruction end = processor.Create(OpCodes.Nop);
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << index));
            processor.Append(processor.Create(OpCodes.And));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << index));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Stloc, index + offset));
            processor.Append(processor.Create(OpCodes.Ldloc, index + offset));
            processor.Append(processor.Create(OpCodes.Brfalse_S, end));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, field.FieldType.FullName));
            processor.Append(processor.Create(OpCodes.Stfld, field));
            processor.Append(end);
        }
    }
}
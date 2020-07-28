using Mono.CecilX;
using Mono.CecilX.Cil;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ServerSyncProcessor
    {
        public static void Weave(ModuleDefinition module, List<TypeDefinition> syncs, out Dictionary<FieldDefinition, MethodDefinition> gets, out Dictionary<FieldDefinition, MethodDefinition> sets)
        {
            sets = new Dictionary<FieldDefinition, MethodDefinition>();
            gets = new Dictionary<FieldDefinition, MethodDefinition>();

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

                FieldDefinition dirty = FieldFactory.CreateField(module, type, "m_Dirty", FieldAttributes.Private, module.ImportReference(typeof(long)));

                //public bool IsSeialize();
                ModifyIsSerializeMethod(module, type, dirty);

                //get set method
                for (int i = 0; i < fields.Count; ++i)
                {
                    FieldDefinition field = fields[i];

                    if (!CheckHelper.CheckSyncFiled(WeaverProgram.Server, type, field.FieldType.Resolve()))
                    {
                        continue;
                    }

                    string name = field.Name;
                    MethodDefinition get = CreateGetMethod(module, type, field, name);
                    MethodDefinition set = CreateSetMethod(module, type, field, name, dirty, i);

                    PropertyDefinition propertyDefinition = new PropertyDefinition("Sync" + name, PropertyAttributes.None, field.FieldType)
                    {
                        GetMethod = get,
                        SetMethod = set
                    };

                    type.Properties.Add(propertyDefinition);

                    gets.Add(field, get);
                    sets.Add(field, set);
                }

                //Serialize
                MethodDefinition serialize = ResolveHelper.ResolveMethod(type, "Serialize");

                ILProcessor processor = serialize.Body.GetILProcessor();
                processor.Body.Instructions.Clear();

                Instruction ins1 = processor.Create(OpCodes.Nop);
                Instruction ins2 = processor.Create(OpCodes.Ldarg_0);
                Instruction ins3 = processor.Create(OpCodes.Ldc_I8, 0L);
                Instruction ins4 = processor.Create(OpCodes.Stfld, dirty);
                Instruction ins5 = processor.Create(OpCodes.Nop);
                Instruction ins6 = processor.Create(OpCodes.Ret);

                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, dirty));
                processor.Append(processor.Create(OpCodes.Ldc_I8, 0L));
                processor.Append(processor.Create(OpCodes.Cgt));
                processor.Append(processor.Create(OpCodes.Stloc_0));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Brfalse, ins6));
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, dirty));
                processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, dirty.FieldType.Resolve()));

                serialize.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

                int offset = serialize.Body.Variables.Count;

                for (int i = 0; i < fields.Count; ++i)
                {
                    WriteSerializeInstruction(module, serialize, processor, fields[i], dirty, i, offset);
                }

                processor.Append(ins1);
                processor.Append(ins2);
                processor.Append(ins3);
                processor.Append(ins4);
                processor.Append(ins5);
                processor.Append(ins6);
            }
        }

        private static void ModifyIsSerializeMethod(ModuleDefinition module, TypeDefinition type, FieldDefinition dirty)
        {
            MethodDefinition method = ResolveHelper.ResolveMethod(type, "IsSerialize");
            method.Body.Variables.Clear();
            method.Body.Instructions.Clear();

            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            ILProcessor processor = method.Body.GetILProcessor();
            Instruction ins = processor.Create(OpCodes.Ldloc_0);
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, dirty));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 0L));
            processor.Append(processor.Create(OpCodes.Cgt));
            processor.Append(processor.Create(OpCodes.Stloc_0));
            processor.Append(processor.Create(OpCodes.Br_S, ins));
            processor.Append(ins);
            processor.Append(processor.Create(OpCodes.Ret));
        }

        private static MethodDefinition CreateGetMethod(ModuleDefinition module, TypeDefinition type, FieldDefinition field, string name)
        {
            MethodDefinition get = MethodFactory.CreateMethod(module,
                                                              type,
                                                              "get_Sync" + name,
                                                              MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                              field.FieldType,
                                                              true);

            ILProcessor processor = get.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Ret));
            get.Body.Variables.Add(new VariableDefinition(field.FieldType));
            get.SemanticsAttributes = MethodSemanticsAttributes.Getter;

            return get;
        }

        public static MethodDefinition CreateSetMethod(ModuleDefinition module, TypeDefinition type, FieldDefinition field, string name, FieldDefinition dirty, int offset)
        {
            MethodDefinition set = MethodFactory.CreateMethod(module,
                                                              type,
                                                              "set_Sync" + name,
                                                              MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                              module.ImportReference(typeof(void)),
                                                              true);

            set.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));
            set.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, field.FieldType));

            ILProcessor processor = set.Body.GetILProcessor();

            if (BaseTypeFactory.IsSystemBaseType(field.FieldType.Resolve()))
            {
                if (field.FieldType.IsValueType)
                {
                    AppendBaseTypeInstruction(processor, field, dirty, offset);
                }
                else if (field.FieldType.ToString() == "System.String")
                {
                    AppendOpInequalityInstruction(module, processor, field, dirty, offset);
                }
            }
            else if(field.FieldType.IsValueType)
            {
                AppendOpInequalityInstruction(module, processor, field, dirty, offset);
            }

            set.SemanticsAttributes = MethodSemanticsAttributes.Setter;

            return set;
        }

        private static void AppendBaseTypeInstruction(ILProcessor processor, FieldDefinition field, FieldDefinition dirty, int offset)
        {
            Instruction end = processor.Create(OpCodes.Ret);
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Stloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(processor.Create(OpCodes.Brfalse_S, end));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Stfld, field));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, dirty));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << offset));
            processor.Append(processor.Create(OpCodes.Or));
            processor.Append(processor.Create(OpCodes.Stfld, dirty));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(end);
        }

        private static void AppendOpInequalityInstruction(ModuleDefinition module, ILProcessor processor, FieldDefinition field, FieldDefinition dirty, int offset)
        {
            Instruction end = processor.Create(OpCodes.Ret);
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Call, module.ImportReference(ResolveHelper.ResolveMethod(field.FieldType, "op_Inequality"))));
            processor.Append(processor.Create(OpCodes.Stloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(processor.Create(OpCodes.Brfalse_S, end));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Stfld, field));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, dirty));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << offset));
            processor.Append(processor.Create(OpCodes.Or));
            processor.Append(processor.Create(OpCodes.Stfld, dirty));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(end);
        }

        private static void WriteSerializeInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, FieldDefinition field, FieldDefinition dirty, int index, int offset)
        {
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            Instruction end = processor.Create(OpCodes.Nop);

            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, dirty));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << index));
            processor.Append(processor.Create(OpCodes.And));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << index));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Stloc, index + offset));
            processor.Append(processor.Create(OpCodes.Ldloc, index + offset));
            processor.Append(processor.Create(OpCodes.Brfalse_S, end));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, field.FieldType.Resolve()));
            processor.Append(end);
        }
    }
}
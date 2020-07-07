using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;

namespace Zyq.Weaver
{
    public static class StructFactory
    {
        public static MethodDefinition CreateSerialize(ModuleDefinition module, TypeDefinition type)
        {
            MethodDefinition serialize = MethodFactory.CreateMethod(module,
                                                                    type,
                                                                    "Serialize",
                                                                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                                    true);

            serialize.Parameters.Add(new ParameterDefinition(module.ImportReference(WeaverProgram.NetworkWriterType)));

            ILProcessor processor = serialize.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Nop));

            Collection<FieldDefinition> fields = type.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                FieldDefinition field = fields[i];
                if (BaseTypeFactory.IsBaseType(field.FieldType.ToString()))
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldfld, field));
                    processor.Append(BaseTypeFactory.CreateWriteTypeInstruction(module, processor, field.FieldType.ToString()));
                    processor.Append(processor.Create(OpCodes.Nop));
                }
                else
                {
                    TypeDefinition fieldType = field.FieldType as TypeDefinition;
                    if (fieldType != null && fieldType.IsValueType)
                    {
                        MethodDefinition ser = CreateSerialize(module, fieldType);
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                        processor.Append(processor.Create(OpCodes.Ldflda, field));
                        processor.Append(processor.Create(OpCodes.Ldarg_1));
                        processor.Append(processor.Create(OpCodes.Call, ser));
                        processor.Append(processor.Create(OpCodes.Nop));
                    }
                }
            }

            processor.Append(processor.Create(OpCodes.Ret));

            return serialize;
        }

        public static MethodDefinition CreateDeserialize(ModuleDefinition module, TypeDefinition type)
        {
            MethodDefinition serialize = MethodFactory.CreateMethod(module,
                                                                    type,
                                                                    "Deserialize",
                                                                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                                    true);

            serialize.Parameters.Add(new ParameterDefinition(module.ImportReference(WeaverProgram.NetworkReaderType)));

            ILProcessor processor = serialize.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Nop));

            Collection<FieldDefinition> fields = type.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                FieldDefinition field = fields[i];
                if (BaseTypeFactory.IsBaseType(field.FieldType.ToString()))
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(BaseTypeFactory.CreateReadTypeInstruction(module, processor, field.FieldType.ToString()));
                    processor.Append(processor.Create(OpCodes.Stfld, field));
                    processor.Append(processor.Create(OpCodes.Nop));
                }
                else
                {
                    /**
                    TypeDefinition fieldType = field.FieldType as TypeDefinition;
                    if (fieldType != null && fieldType.IsValueType)
                    {
                        MethodDefinition ser = CreateSerialize(module, fieldType);
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                        processor.Append(processor.Create(OpCodes.Ldflda, field));
                        processor.Append(processor.Create(OpCodes.Ldarg_1));
                        processor.Append(processor.Create(OpCodes.Call, ser));
                        processor.Append(processor.Create(OpCodes.Nop));
                    }
                    **/
                }
            }

            processor.Append(processor.Create(OpCodes.Ret));

            return serialize;
        }
    }
}
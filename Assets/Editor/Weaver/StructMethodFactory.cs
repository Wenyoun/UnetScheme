﻿using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class StructMethodFactory
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

            foreach (FieldDefinition field in type.Fields)
            {
                if (BaseTypeFactory.IsBaseType(field.FieldType.ToString()))
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldfld, field));
                    processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, field.FieldType.ToString()));
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
                    }
                }
            }

            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ret));

            return serialize;
        }

        public static MethodDefinition CreateDeserialize(ModuleDefinition module, TypeDefinition type)
        {
            MethodDefinition deserialize = MethodFactory.CreateMethod(module,
                                                                      type,
                                                                      "Deserialize",
                                                                      MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                                      true);

            deserialize.Parameters.Add(new ParameterDefinition(module.ImportReference(WeaverProgram.NetworkReaderType)));

            ILProcessor processor = deserialize.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Nop));

            foreach (FieldDefinition field in type.Fields)
            {
                if (BaseTypeFactory.IsBaseType(field.FieldType.ToString()))
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, field.FieldType.ToString()));
                    processor.Append(processor.Create(OpCodes.Stfld, field));
                }
                else
                {
                    TypeDefinition fieldType = field.FieldType as TypeDefinition;
                    if (fieldType != null && fieldType.IsValueType)
                    {
                        MethodDefinition dser = CreateDeserialize(module, fieldType);
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                        processor.Append(processor.Create(OpCodes.Ldflda, field));
                        processor.Append(processor.Create(OpCodes.Ldarg_1));
                        processor.Append(processor.Create(OpCodes.Call, dser));
                    }
                }
            }

            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ret));

            return deserialize;
        }
    }
}
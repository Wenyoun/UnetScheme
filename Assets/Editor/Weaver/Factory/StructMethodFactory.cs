﻿using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class StructMethodFactory
    {
        public static MethodReference FindSerialize(ModuleDefinition module, TypeDefinition type)
        {
            return module.ImportReference(ResolveHelper.ResolveMethod(type, "Serialize"));
        }

        public static MethodReference FindDeserialize(ModuleDefinition module, TypeDefinition type)
        {
            return module.ImportReference(ResolveHelper.ResolveMethod(type, "Deserialize"));
        }

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
                TypeDefinition fieldType = field.FieldType.Resolve();

                if (field.FieldType.IsArray)
                {
                    ArrayWriteFactory.CreateStructFieldWriteInstruction(module, serialize, processor, field, fieldType);
                }
                else if (BaseTypeFactory.IsBaseType(fieldType.ToString()) || fieldType.IsEnum)
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldfld, field));
                    if (fieldType.IsEnum)
                    {
                        processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));
                    }
                    else
                    {
                        processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, fieldType.ToString()));
                    }
                }
                else if (fieldType.IsValueType)
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldflda, field));
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Call, CreateSerialize(module, fieldType)));
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
                TypeDefinition fieldType = field.FieldType.Resolve();

                if (field.FieldType.IsArray)
                {
                    ArrayReadFactory.CreateStructFieldReadInstruction(module, deserialize, processor, field, fieldType);
                }
                else if (BaseTypeFactory.IsBaseType(fieldType.ToString()) || fieldType.IsEnum)
                {
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    if (fieldType.IsEnum)
                    {
                        processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
                    }
                    else
                    {
                        processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, fieldType.ToString()));
                    }
                    processor.Append(processor.Create(OpCodes.Stfld, field));
                }
                else if (fieldType.IsValueType)
                {
                    MethodDefinition dser = CreateDeserialize(module, fieldType);
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Ldflda, field));
                    processor.Append(processor.Create(OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Call, dser));
                }
            }

            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ret));

            return deserialize;
        }
    }
}
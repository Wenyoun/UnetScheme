using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class ArrayReadFactory
    {
        public static void CreateStructReadInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, FieldDefinition field, TypeDefinition fieldType)
        {
            int lenIndex = method.Body.Variables.Count;
            int intIndex = method.Body.Variables.Count + 1;
            int boolIndex = method.Body.Variables.Count + 2;

            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            //int len = reader.ReadInt32();
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //T[] this.Arr = new T[len];
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Newarr, module.ImportReference(fieldType)));
            processor.Append(processor.Create(OpCodes.Stfld, module.ImportReference(field)));

            //for (int i = 0; i < len; i++)
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            Instruction start = processor.Create(OpCodes.Nop);
            Instruction check = processor.Create(OpCodes.Nop);

            //go to check
            processor.Append(processor.Create(OpCodes.Br, check));
            processor.Append(start);

            if (BaseTypeFactory.IsBaseType(fieldType.ToString()) || fieldType.IsEnum)
            {
                //this.Arr[i] = reader.ReadT();
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, field));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(fieldType)));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                if (fieldType.IsEnum)
                {
                    processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
                }
                else
                {
                    processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, fieldType.ToString()));
                }
                processor.Append(processor.Create(OpCodes.Stobj, module.ImportReference(fieldType)));
            }
            else if (fieldType.IsValueType)
            {
                //this.Arr[i].Deserialize(writer);
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, field));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(fieldType)));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindDeserialize(module, fieldType)));
            }

            //i++
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_1));
            processor.Append(processor.Create(OpCodes.Add));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //i < len
            processor.Append(check);
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Clt));
            processor.Append(processor.Create(OpCodes.Stloc, boolIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, boolIndex));

            //go to start
            processor.Append(processor.Create(OpCodes.Brtrue, start));
        }

        public static void CreateParamReadInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, TypeDefinition parmType)
        {
            int typeIndex = method.Body.Variables.Count - 1;
            int lenIndex = method.Body.Variables.Count;
            int intIndex = method.Body.Variables.Count + 1;
            int boolIndex = method.Body.Variables.Count + 2;

            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            //int len = reader.ReadInt32();
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //T[] arr = new T[len];
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Newarr, module.ImportReference(parmType)));
            processor.Append(processor.Create(OpCodes.Stloc, typeIndex));

            //for (int i = 0; i < len; i++)
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            Instruction start = processor.Create(OpCodes.Nop);
            Instruction check = processor.Create(OpCodes.Nop);

            //go to check
            processor.Append(processor.Create(OpCodes.Br, check));
            processor.Append(start);

            if (BaseTypeFactory.IsBaseType(parmType.ToString()) || parmType.IsEnum)
            {
                //arr[i] = reader.ReadT();
                processor.Append(processor.Create(OpCodes.Ldloc, typeIndex));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(parmType)));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                if (parmType.IsEnum)
                {
                    processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
                }
                else
                {
                    processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, parmType.ToString()));
                }
                processor.Append(processor.Create(OpCodes.Stobj, module.ImportReference(parmType)));
            }
            else if (parmType.IsValueType)
            {
                //arr[i].Deserialize(writer);
                processor.Append(processor.Create(OpCodes.Ldloc, typeIndex));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(parmType)));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindDeserialize(module, parmType)));
            }

            //i++
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_1));
            processor.Append(processor.Create(OpCodes.Add));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //i < len
            processor.Append(check);
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Clt));
            processor.Append(processor.Create(OpCodes.Stloc, boolIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, boolIndex));

            //go to start
            processor.Append(processor.Create(OpCodes.Brtrue, start));
        }
    }
}
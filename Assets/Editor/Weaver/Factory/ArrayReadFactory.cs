using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class ArrayReadFactory
    {
        public static void CreateStructFieldReadInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, FieldDefinition field, TypeDefinition fieldType)
        {
            int lenIndex = method.Body.Variables.Count;
            int checkIndex = method.Body.Variables.Count + 1;
            int intIndex = method.Body.Variables.Count + 2;
            int boolIndex = method.Body.Variables.Count + 3;

            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            Instruction goto1 = processor.Create(OpCodes.Nop);
            Instruction goto2 = processor.Create(OpCodes.Nop);
            Instruction goto3 = processor.Create(OpCodes.Nop);

            //int len = reader.ReadInt32();
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //if (len > 0)
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Cgt));
            processor.Append(processor.Create(OpCodes.Stloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Brfalse_S, goto3));

            //T[] this.Field = new T[len];
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Newarr, module.ImportReference(fieldType)));
            processor.Append(processor.Create(OpCodes.Stfld, module.ImportReference(field)));

            //for (int i = 0; i < len; i++)
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));


            //go to check
            processor.Append(processor.Create(OpCodes.Br_S, goto2));
            processor.Append(goto1);

            if (BaseTypeFactory.IsBaseType(fieldType))
            {
                //this.Field[i] = reader.ReadT();
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, field));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(fieldType)));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, fieldType));
                processor.Append(processor.Create(OpCodes.Stobj, module.ImportReference(fieldType)));
            }
            else if (fieldType.IsValueType)
            {
                //this.Field[i].Deserialize(writer);
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
            processor.Append(goto2);
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Clt));
            processor.Append(processor.Create(OpCodes.Stloc, boolIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, boolIndex));

            //go to start
            processor.Append(processor.Create(OpCodes.Brtrue_S, goto1));
            processor.Append(goto3);
        }

        public static void CreateMethodVariableReadInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, TypeDefinition parmType)
        {
            int typeIndex = method.Body.Variables.Count - 1;
            int lenIndex = method.Body.Variables.Count;
            int checkIndex = method.Body.Variables.Count + 1;
            int intIndex = method.Body.Variables.Count + 2;
            int boolIndex = method.Body.Variables.Count + 3;

            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            Instruction goto1 = processor.Create(OpCodes.Nop);
            Instruction goto2 = processor.Create(OpCodes.Nop);
            Instruction goto3 = processor.Create(OpCodes.Nop);

            //T[] varArray = null;
            processor.Append(processor.Create(OpCodes.Ldnull));
            processor.Append(processor.Create(OpCodes.Stloc, typeIndex));

            //int len = reader.ReadInt32();
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, typeof(int).ToString()));
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //if (len > 0)
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Cgt));
            processor.Append(processor.Create(OpCodes.Stloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Brfalse_S, goto3));

            //varArray = new T[len];
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Newarr, module.ImportReference(parmType)));
            processor.Append(processor.Create(OpCodes.Stloc, typeIndex));

            //for (int i = 0; i < len; i++)
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //go to check
            processor.Append(processor.Create(OpCodes.Br_S, goto2));
            processor.Append(goto1);

            if (BaseTypeFactory.IsBaseType(parmType))
            {
                //varArray[i] = reader.ReadT();
                processor.Append(processor.Create(OpCodes.Ldloc, typeIndex));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(parmType)));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(BaseTypeFactory.CreateReadInstruction(module, processor, parmType));
                processor.Append(processor.Create(OpCodes.Stobj, module.ImportReference(parmType)));
            }
            else if (parmType.IsValueType)
            {
                //varArray[i].Deserialize(writer);
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
            processor.Append(goto2);
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Clt));
            processor.Append(processor.Create(OpCodes.Stloc, boolIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, boolIndex));

            //go to start
            processor.Append(processor.Create(OpCodes.Brtrue_S, goto1));
            processor.Append(goto3);
        }
    }
}
using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class ArrayWriteFactory
    {
        public static void CreateStructFieldWriteInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, FieldDefinition field, TypeDefinition fieldType)
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
            Instruction goto4 = processor.Create(OpCodes.Nop);
            Instruction goto5 = processor.Create(OpCodes.Nop);

            //int len = this.Field != null ? this.Field.Length : 0;
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Brtrue_S, goto1));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Br_S, goto2));
            processor.Append(goto1);
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Ldlen));
            processor.Append(processor.Create(OpCodes.Conv_I4));
            processor.Append(goto2);
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //writer.Write(len);
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));

            //if (len > 0)
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Cgt));
            processor.Append(processor.Create(OpCodes.Stloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Brfalse_S, goto5));

            //for (int i = 0; i < len; i++)
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //go to check
            processor.Append(processor.Create(OpCodes.Br_S, goto4));
            processor.Append(goto3);

            if (BaseTypeFactory.IsBaseType(fieldType.ToString()) || fieldType.IsEnum)
            {
                //writer.Write(this.Field[i]);
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, field));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(fieldType)));
                processor.Append(processor.Create(OpCodes.Ldobj, module.ImportReference(fieldType)));
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
                //this.Field[i].Serialize(writer);
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Ldfld, field));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(fieldType)));
                processor.Append(processor.Create(OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindSerialize(module, fieldType)));
            }

            //i++
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_1));
            processor.Append(processor.Create(OpCodes.Add));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //i < len
            processor.Append(goto4);
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Clt));
            processor.Append(processor.Create(OpCodes.Stloc, boolIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, boolIndex));

            //go to start
            processor.Append(processor.Create(OpCodes.Brtrue_S, goto3));
            processor.Append(goto5);
        }

        public static void CreateMethodParamWriteInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, TypeDefinition parmType, byte argIndex)
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
            Instruction goto4 = processor.Create(OpCodes.Nop);
            Instruction goto5 = processor.Create(OpCodes.Nop);

            //int len = param != null ? param.Length : 0;
            processor.Append(processor.Create(OpCodes.Ldarg_S, argIndex));
            processor.Append(processor.Create(OpCodes.Brtrue_S, goto1));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Br_S, goto2));
            processor.Append(goto1);
            processor.Append(processor.Create(OpCodes.Ldarg_S, argIndex));
            processor.Append(processor.Create(OpCodes.Ldlen));
            processor.Append(processor.Create(OpCodes.Conv_I4));
            processor.Append(goto2);
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //writer.Write(len);
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));

            //if (len > 0)
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Cgt));
            processor.Append(processor.Create(OpCodes.Stloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, checkIndex));
            processor.Append(processor.Create(OpCodes.Brfalse_S, goto5));

            //for (int i = 0; i < len; i++)
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //go to check
            processor.Append(processor.Create(OpCodes.Br_S, goto4));
            processor.Append(goto3);

            if (BaseTypeFactory.IsBaseType(parmType.ToString()) || parmType.IsEnum)
            {
                //writer.Write(param[i]);
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Ldarg_S, argIndex));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(parmType)));
                processor.Append(processor.Create(OpCodes.Ldobj, module.ImportReference(parmType)));
                if (parmType.IsEnum)
                {
                    processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));
                }
                else
                {
                    processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, parmType.ToString()));
                }
            }
            else if (parmType.IsValueType)
            {
                //param[i].Serialize(writer);
                processor.Append(processor.Create(OpCodes.Ldarg_S, argIndex));
                processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
                processor.Append(processor.Create(OpCodes.Ldelema, module.ImportReference(parmType)));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindSerialize(module, parmType)));
            }

            //i++
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldc_I4_1));
            processor.Append(processor.Create(OpCodes.Add));
            processor.Append(processor.Create(OpCodes.Stloc, intIndex));

            //i < len
            processor.Append(goto4);
            processor.Append(processor.Create(OpCodes.Ldloc, intIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(processor.Create(OpCodes.Clt));
            processor.Append(processor.Create(OpCodes.Stloc, boolIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, boolIndex));

            //go to start
            processor.Append(processor.Create(OpCodes.Brtrue_S, goto3));
            processor.Append(goto5);
        }
    }
}
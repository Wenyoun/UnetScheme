using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class ArrayFactory
    {
        public static void CreateWriteInstruction(ModuleDefinition module, MethodDefinition method, ILProcessor processor, TypeDefinition parmType, byte argIndex)
        {
            int lenIndex = method.Body.Variables.Count;
            int intIndex = method.Body.Variables.Count + 1;
            int boolIndex = method.Body.Variables.Count + 2;

            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(int))));
            method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));

            //int len = v.Length;
            processor.Append(processor.Create(OpCodes.Ldarg_S, argIndex));
            processor.Append(processor.Create(OpCodes.Ldlen));
            processor.Append(processor.Create(OpCodes.Conv_I4));
            processor.Append(processor.Create(OpCodes.Stloc, lenIndex));

            //writer.Write(len);
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc, lenIndex));
            processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, typeof(int).ToString()));

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
                //writer.Write(v[i]);
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
                //v[i].Serialize(writer);
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
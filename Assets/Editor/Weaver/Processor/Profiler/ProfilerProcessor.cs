using Base;
using Mono.CecilX;
using Mono.CecilX.Cil;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class ProfilerProcessor
    {
        public static void Weave(ModuleDefinition module)
        {
            List<TypeDefinition> types = new List<TypeDefinition>();

            MethodReference begin = module.ImportReference(typeof(ProfilerManager).GetMethod("Begin", new System.Type[] { typeof(string) }));
            MethodReference end = module.ImportReference(typeof(ProfilerManager).GetMethod("End", new System.Type[] { typeof(string) }));

            foreach (TypeDefinition type in module.Types)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    string methodName = method.Name;
                    if (methodName.IndexOf("get_") == -1 &&
                        methodName.IndexOf("set_") == -1 &&
                        methodName.IndexOf(".cctor") == -1 &&
                        methodName.IndexOf(".ctor") == -1)
                    {
                        if (method.Body.Instructions.Count <= 2)
                        {
                            continue;
                        }

                        string name = type.Name + "." + methodName;
                        ILProcessor processor = method.Body.GetILProcessor();

                        Instruction first = method.Body.Instructions[0];
                        Instruction last = method.Body.Instructions[method.Body.Instructions.Count - 1];

                        Instruction nop = processor.Create(OpCodes.Nop);

                        foreach (Instruction ins in method.Body.Instructions)
                        {
                            if (ins.OpCode == OpCodes.Brfalse_S ||
                                ins.OpCode == OpCodes.Brfalse ||
                                ins.OpCode == OpCodes.Brtrue_S ||
                                ins.OpCode == OpCodes.Brtrue ||
                                ins.OpCode == OpCodes.Br_S ||
                                ins.OpCode == OpCodes.Br)
                            {
                                Instruction target = (Instruction)ins.Operand;
                                if (target.OpCode == OpCodes.Ret)
                                {
                                    ins.Operand = nop;
                                }
                            }
                        }

                        processor.InsertBefore(first, processor.Create(OpCodes.Ldstr, name));
                        processor.InsertBefore(first, processor.Create(OpCodes.Call, begin));

                        processor.InsertBefore(last, nop);
                        processor.InsertBefore(last, processor.Create(OpCodes.Ldstr, name));
                        processor.InsertBefore(last, processor.Create(OpCodes.Call, end));
                    }
                }
            }
        }
    }
}
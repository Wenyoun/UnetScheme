using Mono.CecilX;
using Zyq.Game.Base;
using Mono.CecilX.Cil;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class ProfilerProcessor
    {
        public static void Weave(ModuleDefinition module)
        {
            MethodReference begin = module.ImportReference(typeof(ProfilerManager).GetMethod("Begin", new System.Type[] { typeof(string) }));
            MethodReference end = module.ImportReference(typeof(ProfilerManager).GetMethod("End", new System.Type[] { typeof(string) }));

            List<TypeDefinition> types = new List<TypeDefinition>();

            foreach (TypeDefinition type in module.Types)
            {
                if (!type.IsInterface && type.FullName != typeof(ProfilerManager).FullName)
                {
                    types.Add(type);
                }
            }

            foreach (TypeDefinition type in types)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.IsAbstract ||
                        method.IsGetter ||
                        method.IsSetter ||
                        method.IsConstructor ||
                        !method.HasBody ||
                        method.Body.Instructions.Count <= 2)
                    {
                        continue;
                    }

                    ILProcessor processor = method.Body.GetILProcessor();

                    MethodReference beginRef = method.Body.Instructions[1].Operand as MethodReference;
                    if (beginRef != null && beginRef.FullName == begin.FullName)
                    {
                        continue;
                    }

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
                            Instruction target = ins.Operand as Instruction;
                            if (target != null && target.OpCode == OpCodes.Ret)
                            {
                                ins.Operand = nop;
                            }
                        }
                    }

                    string name = type.FullName + "." + method.Name;

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
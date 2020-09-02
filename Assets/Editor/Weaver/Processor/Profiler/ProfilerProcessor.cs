using Mono.CecilX;
using Zyq.Game.Base;
using Mono.CecilX.Cil;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class ProfilerProcessor
    {
        private static List<string> BlackList = new List<string>()
        {
            typeof(ProfilerManager).FullName,
        };
        
        public static void Weave(ModuleDefinition module)
        {
            return;
            MethodReference begin = module.ImportReference(typeof(ProfilerManager).GetMethod("BeginSample", new System.Type[] { typeof(string) }));
            MethodReference end = module.ImportReference(typeof(ProfilerManager).GetMethod("EndSample", new System.Type[] { typeof(string) }));

            List<TypeDefinition> types = new List<TypeDefinition>();

            foreach (TypeDefinition type in module.Types)
            {
                if (type.IsInterface ||
                    BlackList.Contains(type.FullName))
                {
                    continue;
                }

                types.Add(type);
            }

            foreach (TypeDefinition type in types)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    string methodName = method.Name;
                    if (method.IsAbstract ||
                        method.IsConstructor ||
                        methodName.IndexOf("get_") >= 0 ||
                        methodName.IndexOf("set_") >= 0 ||
                        methodName.IndexOf(".cctor") >= 0 ||
                        methodName.IndexOf(".ctor") >= 0 ||
                        !method.HasBody ||
                        method.Body.Instructions.Count <= 2)
                    {
                        continue;
                    }

                    string name = type.FullName + "." + methodName;
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
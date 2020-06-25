using UnityEngine;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Zyq.Game.Base;

namespace Zyq.Weaver
{
    public static class ServerReplaceProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<FieldDefinition, MethodDefinition> gets, Dictionary<FieldDefinition, MethodDefinition> sets)
        {
            List<TypeDefinition> defs = new List<TypeDefinition>();

            foreach (TypeDefinition type in module.Types)
            {
                bool find = false;
                foreach (InterfaceImplementation iface in type.Interfaces)
                {
                    if (iface.InterfaceType.FullName == typeof(ISyncAttribute).FullName)
                    {
                        find = true;
                        break;
                    }
                }

                foreach (TypeDefinition nested in type.NestedTypes)
                {
                    defs.Add(nested);
                }

                if (type.IsClass && !find)
                {
                    defs.Add(type);
                }
            }

            foreach (TypeDefinition type in defs)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.Body != null && method.Body.Instructions != null)
                    {
                        for (int i = 0; i < method.Body.Instructions.Count; ++i)
                        {
                            Instruction ins = method.Body.Instructions[i];
                            if (ins.OpCode == OpCodes.Ldfld && ins.Operand is FieldDefinition getField)
                            {
                                if (gets.TryGetValue(getField, out MethodDefinition replacement))
                                {
                                    ins.OpCode = OpCodes.Call;
                                    ins.Operand = replacement;
                                }
                            }
                            else if (ins.OpCode == OpCodes.Stfld && ins.Operand is FieldDefinition setField)
                            {
                                if (sets.TryGetValue(setField, out MethodDefinition replacement))
                                {
                                    ins.OpCode = OpCodes.Call;
                                    ins.Operand = replacement;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
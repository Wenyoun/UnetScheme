using Mono.CecilX;
using Mono.CecilX.Cil;
using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ServerReplaceProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<FieldDefinition, MethodDefinition> gets, Dictionary<FieldDefinition, MethodDefinition> sets)
        {
            List<TypeDefinition> types = new List<TypeDefinition>();

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
                    types.Add(nested);
                }

                if (type.IsClass && !find)
                {
                    types.Add(type);
                }
            }

            foreach (TypeDefinition type in types)
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
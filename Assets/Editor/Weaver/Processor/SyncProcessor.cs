using UnityEngine;
using System;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Weaver
{
    public static class SyncProcessor
    {
        public static void Weave(ModuleDefinition module, List<TypeDefinition> syncs)
        {
            foreach (TypeDefinition type in syncs)
            {
                int index = 0;
                FieldDefinition mask = new FieldDefinition("m_Dirty", FieldAttributes.Private, module.ImportReference(typeof(long)));
                type.Fields.Add(mask);

                foreach (FieldDefinition field in type.Fields)
                {
                    if (field.CustomAttributes.Count > 0 && field.CustomAttributes[0].AttributeType.FullName == WeaverProgram.SyncFieldType.FullName)
                    {
                        string name = field.Name;
                        MethodDefinition get = CreateGetMethod(type, field, name);
                        MethodDefinition set = CreateSetMethod(module, type, field, name, mask, index);

                        PropertyDefinition propertyDefinition = new PropertyDefinition("Sync" + name, PropertyAttributes.None, field.FieldType)
                        {
                            GetMethod = get,
                            SetMethod = set
                        };

                        type.Methods.Add(get);
                        type.Methods.Add(set);
                        type.Properties.Add(propertyDefinition);

                        index = index + 1;
                    }
                }
            }
        }

        private static MethodDefinition CreateGetMethod(TypeDefinition type, FieldDefinition field, string name)
        {
            MethodDefinition get = new MethodDefinition("get_Sync" + name, MethodAttributes.Public |
                                                                           MethodAttributes.SpecialName |
                                                                           MethodAttributes.HideBySig, field.FieldType);
            ILProcessor processor = get.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Ret));
            get.Body.Variables.Add(new VariableDefinition(field.FieldType));
            get.Body.InitLocals = true;
            get.SemanticsAttributes = MethodSemanticsAttributes.Getter;
            return get;
        }

        public static MethodDefinition CreateSetMethod(ModuleDefinition module, TypeDefinition type, FieldDefinition field, string name, FieldDefinition mask, int offset)
        {
            MethodDefinition set = new MethodDefinition("set_Sync" + name, MethodAttributes.Public |
                                                                           MethodAttributes.SpecialName |
                                                                           MethodAttributes.HideBySig, module.ImportReference(typeof(void)));

            set.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(bool))));
            set.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, field.FieldType));

            ILProcessor processor = set.Body.GetILProcessor();
            Instruction end = processor.Create(OpCodes.Ret);

            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Ldc_I4_0));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Stloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc_0));
            processor.Append(processor.Create(OpCodes.Brfalse_S, end));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Stfld, field));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldfld, mask));
            processor.Append(processor.Create(OpCodes.Ldc_I8, 1L << offset));
            processor.Append(processor.Create(OpCodes.Or));
            processor.Append(processor.Create(OpCodes.Stfld, mask));
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(end);

            set.SemanticsAttributes = MethodSemanticsAttributes.Setter;

            return set;
        }
    }
}
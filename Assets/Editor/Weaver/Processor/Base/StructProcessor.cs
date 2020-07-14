using Mono.CecilX;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class StructProcessor
    {
        public static bool Weave(ModuleDefinition module)
        {
            List<TypeDefinition> structs = new List<TypeDefinition>();

            foreach (TypeDefinition type in module.Types)
            {
                if (!type.IsEnum &&
                    type.IsValueType &&
                    type.ToString().IndexOf("Zyq.Game.Base.Protocol") >= 0)
                {
                    structs.Add(type);
                }
            }

            foreach (TypeDefinition type in structs)
            {
                StructMethodFactory.CreateSerialize(module, type);
                StructMethodFactory.CreateDeserialize(module, type);
            }

            return structs.Count > 0;
        }
    }
}
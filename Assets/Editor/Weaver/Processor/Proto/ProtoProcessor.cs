using Mono.CecilX;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class ProtoProcessor
    {
        public static bool Weave(ModuleDefinition module)
        {
            List<TypeDefinition> structs = new List<TypeDefinition>();

            foreach (TypeDefinition type in module.Types)
            {
                if (!type.IsEnum &&
                    type.IsValueType &&
                    type.Namespace == "Nice.Game.Proto")
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
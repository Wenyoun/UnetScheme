using Mono.CecilX;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public class BaseWeaver
    {
        public static bool Weave(ModuleDefinition module)
        {
            return StructProcessor.Weave(module);
        }
    }
}
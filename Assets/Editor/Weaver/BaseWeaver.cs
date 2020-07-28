using Mono.CecilX;

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
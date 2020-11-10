using Mono.CecilX;

namespace Zyq.Weaver
{
    public class ProtoWeaver
    {
        public static bool Weave(ModuleDefinition module)
        {
            return ProtoProcessor.Weave(module);
        }
    }
}
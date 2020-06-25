using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class MethodFactory
    {
        public static MethodDefinition CreateMethod(ModuleDefinition module, TypeDefinition type, string methodName, MethodAttributes attributes)
        {
            MethodDefinition method = ResolveHelper.ResolveMethod(type, methodName);
            if (method == null)
            {
                method = new MethodDefinition(methodName, attributes, module.ImportReference(typeof(void)));
                type.Methods.Add(method);
            }

            method.Parameters.Clear();
            method.Body.Variables.Clear();
            method.Body.Instructions.Clear();

            ILProcessor processor = method.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Ret));

            return method;
        }
    }
}
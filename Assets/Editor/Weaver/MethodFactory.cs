using Mono.CecilX;
using Mono.CecilX.Cil;

namespace Zyq.Weaver
{
    public static class MethodFactory
    {
        public static MethodDefinition CreateMethod(ModuleDefinition module, TypeDefinition type, string methodName, MethodAttributes attributes, bool isEmpty = false)
        {
            return CreateMethod(module, type, methodName, attributes, module.ImportReference(typeof(void)), isEmpty);
        }

        public static MethodDefinition CreateMethod(ModuleDefinition module, TypeDefinition type, string methodName, MethodAttributes attributes, TypeReference returnType, bool isEmpty = false)
        {
            MethodDefinition method = ResolveHelper.ResolveMethod(type, methodName);
            if (method == null)
            {
                method = new MethodDefinition(methodName, attributes, returnType);
                method.Body.InitLocals = true;
                type.Methods.Add(method);
            }

            method.Parameters.Clear();
            method.Body.Variables.Clear();
            method.Body.Instructions.Clear();

            if (!isEmpty)
            {
                ILProcessor processor = method.Body.GetILProcessor();
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ret));
            }

            return method;
        }
    }
}
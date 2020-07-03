using Mono.CecilX;

namespace Zyq.Weaver
{
    public static class ResolveHelper
    {
        public static MethodDefinition ResolveMethod(TypeDefinition type, string methodName)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.Name == methodName)
                {
                    return method;
                }
            }
            return null;
        }

        public static MethodDefinition ResolveMethod(TypeReference type, string methodName)
        {
            foreach (MethodDefinition method in type.Resolve().Methods)
            {
                if (method.Name == methodName)
                {
                    return method;
                }
            }
            return null;
        }

        public static FieldDefinition ResolveField(TypeReference type, string fieldName)
        {
            foreach (FieldDefinition field in type.Resolve().Fields)
            {
                if (field.Name == fieldName)
                {
                    return field;
                }
            }
            return null;
        }
    }
}
using System;
using Mono.CecilX;

namespace Zyq.Weaver
{
    public static class Extensions
    {
        public static TypeReference MakeGenericType(this TypeReference self, params TypeReference[] arguments)
        {
            if (self.GenericParameters.Count != arguments.Length)
            {
                throw new ArgumentException();
            }

            GenericInstanceType instance = new GenericInstanceType(self);
            foreach (TypeReference argument in arguments)
            {
                instance.GenericArguments.Add(argument);
            }

            return instance;
        }

        public static MethodReference MakeGenericMethod(this MethodReference self, params TypeReference[] arguments)
        {
            if (self.GenericParameters.Count != arguments.Length)
            {
                throw new ArgumentException();
            }

            GenericInstanceMethod instance = new GenericInstanceMethod(self);
            foreach (TypeReference argument in arguments)
            {
                instance.GenericArguments.Add(argument);
            }

            return instance;
        }

        public static MethodReference MakeGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            MethodReference reference = new MethodReference(self.Name, self.ReturnType)
            {
                DeclaringType = self.DeclaringType.MakeGenericType(arguments),
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention,
            };

            foreach (ParameterDefinition parameter in self.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }

            foreach (GenericParameter parameter in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(parameter.Name, reference));
            }

            return reference;
        }

        public static FieldReference MakeGeneric(this FieldReference self, params TypeReference[] arguments)
        {
            return new FieldReference(
                self.Name,
                self.DeclaringType.MakeGenericType(arguments),
                self.FieldType);
        }
    }
}
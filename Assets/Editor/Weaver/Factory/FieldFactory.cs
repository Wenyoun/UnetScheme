using Mono.CecilX;

namespace Zyq.Weaver
{
    public static class FieldFactory
    {
        public static FieldDefinition CreateField(ModuleDefinition module, TypeDefinition type, string fieldName, FieldAttributes attributes, TypeReference fieldType)
        {
            FieldDefinition field = ResolveHelper.ResolveField(type, fieldName);
            if (field == null)
            {
                field = new FieldDefinition(fieldName, attributes, fieldType);
                type.Fields.Add(field);
            }
            return field;
        }
    }
}
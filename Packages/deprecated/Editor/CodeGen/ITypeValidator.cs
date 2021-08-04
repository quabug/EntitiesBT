using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variant;

namespace EntitiesBT.Editor
{
    public interface ITypeValidator
    {
        bool Verify(Type value);
    }

    [Serializable]
    public class TypeValidatorWithFullName : ITypeValidator
    {
        public string[] TypeFullNames = {};

        public bool Verify(Type type)
        {
            return TypeFullNames.Contains(type.FullName);
        }
    }

    public class TypeValidatorWithoutBlobVariant : ITypeValidator
    {
        public bool Verify(Type node)
        {
            var fields = node.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return !fields.Select(fi => fi.FieldType)
                .Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BlobVariantRO<>))
            ;
        }
    }
}

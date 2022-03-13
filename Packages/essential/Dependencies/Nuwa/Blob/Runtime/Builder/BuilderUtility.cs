using System;
using System.Reflection;

namespace Nuwa.Blob
{
    public static class BuilderUtility
    {
        public static void SetBlobDataType(Type blobType, ref IBuilder[] builders, ref string[] fieldNames)
        {
#if UNITY_EDITOR
            var blobFields = blobType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? Array.Empty<FieldInfo>();
            Array.Resize(ref builders, blobFields.Length);
            Array.Resize(ref fieldNames, blobFields.Length);
            for (var i = 0; i < blobFields.Length; i++)
            {
                var blobField = blobFields[i];
                var builderFactory = blobField.FindBuilderCreator();
                var fieldBuilder = builders[i];
                if (fieldBuilder == null || fieldBuilder.GetType() != builderFactory.Type || blobField.Name != fieldNames[i])
                {
                    var builderIndex = Array.IndexOf(fieldNames, blobField.Name);
                    if (builderIndex >= 0) builders[i] = builders[builderIndex];
                    else if (fieldBuilder != null && fieldBuilder.GetType() == builderFactory.Type) builders[i] = fieldBuilder;
                    else builders[i] = (IBuilder)builderFactory.Create();
                    fieldNames[i] = blobField.Name;
                }
            }
#endif
        }

    }
}
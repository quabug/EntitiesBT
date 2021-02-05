using System;
using System.Reflection;
using EntitiesBT.Editor;
using EntitiesBT.Variant;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual.Editor
{
    public class VisualCodeGenBlobArray : INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            return fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(BlobArray<>);
        }

        public string GenerateField(FieldInfo fi)
        {
            return $"public {fi.FieldType.GenericTypeArguments[0].FullName}[] {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"blobBuilder.AllocateArray(ref data.{fi.Name}, {fi.Name});";
        }
    }

    public class VisualCodeGeneratorBlobString: INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            return fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(BlobString);
        }

        public string GenerateField(FieldInfo fi)
        {
            return $"[PortDescription(Runtime.ValueType.StringReference)] public InputDataPort {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"blobBuilder.AllocateString(ref data.{fi.Name}, instance.ReadString(@this.{fi.Name}));";
        }
    }

    [Serializable]
    public class VisualCodeGeneratorBlobVariantField : INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            if (!fi.FieldType.IsGenericType) return false;
            var type = fi.FieldType.GetGenericTypeDefinition();
            return type == typeof(BlobVariantReader<>)
                   || type == typeof(BlobVariantWriter<>)
                   || type == typeof(BlobVariantReaderAndWriter<>)
               ;
        }

        public string GenerateField(FieldInfo fi)
        {
            var type = fi.FieldType.GenericTypeArguments[0];
            var valueType = type.ToRunTimeValueType();
            var variantType = fi.FieldType.GetGenericTypeDefinition();
            if (variantType == typeof(BlobVariantReader<>))
                return $"[PortDescription(Runtime.ValueType.{valueType})] public InputDataPort {fi.Name};";
            if (variantType == typeof(BlobVariantWriter<>))
                return $"[PortDescription(Runtime.ValueType.{valueType})] public OutputDataPort {fi.Name};";

            return $"[PortDescription(Runtime.ValueType.{valueType}, \"{fi.Name}\")] public InputDataPort Input{fi.Name};"
                + Environment.NewLine + "        "
                + $"[PortDescription(Runtime.ValueType.{valueType}, \"{fi.Name}\")] public OutputDataPort Output{fi.Name};"
                + Environment.NewLine + "        "
                + $"public bool IsLinked{fi.Name};"
            ;
        }

        public string GenerateBuild(FieldInfo fi)
        {
            var type = fi.FieldType.GenericTypeArguments[0];
            if (type == typeof(BlobVariantReader<>)) return BuildScript(fi.Name, fi.Name, "Reader");
            if (type == typeof(BlobVariantWriter<>)) return BuildScript(fi.Name, fi.Name, "Writer");
            return $"new {nameof(DataPortReaderAndWriter)}(@this.IsLinked{fi.Name}, @this.Input{fi.Name}, @this.Output{fi.Name}).ToVariantReaderAndWriter<{type.FullName}>(instance, definition).Allocate(ref blobBuilder, ref data.{fi.Name}, self, builders);";

            string BuildScript(string fieldName, string dataFieldName, string variantSuffix)
            {
                return $"@this.{fieldName}.ToVariant{variantSuffix}<{type.FullName}>(instance, definition).Allocate(ref blobBuilder, ref data.{dataFieldName}, self, builders);";
            }
        }
    }
}

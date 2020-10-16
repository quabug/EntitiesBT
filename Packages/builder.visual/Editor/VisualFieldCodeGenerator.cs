using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using EntitiesBT.Variable;
using Unity.Entities;
using UnityEditor;

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
    public class VisualCodeGeneratorBlobVariableField : INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            return fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(BlobVariableReader<>);
        }

        public string GenerateField(FieldInfo fi)
        {
            var type = fi.FieldType.GenericTypeArguments[0];
            var valueType = type.ToRunTimeValueType();
            return $"[PortDescription(Runtime.ValueType.{valueType})] public InputDataPort {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            var type = fi.FieldType.GenericTypeArguments[0];
            var isReadOnly = fi.GetCustomAttribute<ReadOnlyAttribute>() != null;
            return $"@this.{fi.Name}.ToVariableProperty{(isReadOnly ? "ReadOnly" : "ReadWrite")}<{type.FullName}>(instance, definition).Allocate(ref blobBuilder, ref data.{fi.Name}, self, builders);";
        }
    }
}

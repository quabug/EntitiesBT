using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEditor;
using UnityEditorInternal;

namespace EntitiesBT.Editor
{
    public class BlobArrayFieldCodeGenerator : INodeDataFieldCodeGenerator
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
            return $"builder.AllocateArray(ref data.{fi.Name}, {fi.Name});";
        }
    }

    public class BlobStringFieldCodeGenerator : INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            return fi.FieldType == typeof(BlobString);
        }

        public string GenerateField(FieldInfo fi)
        {
            return $"public string {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"builder.AllocateString(ref data.{fi.Name}, {fi.Name});";
        }
    }

    public class BlobVariantFieldCodeGenerator : INodeDataFieldCodeGenerator
    {
        public string VariantInterfaceNamespace = "EntitiesBT.Variant";

        public bool ShouldGenerate(FieldInfo fi)
        {
            if (!fi.FieldType.IsGenericType) return false;
            var variantType = fi.FieldType.GetGenericTypeDefinition();
            if (variantType == typeof(BlobVariantReader<>)) return true;
            if (variantType == typeof(BlobVariantWriter<>)) return true;
            if (variantType == typeof(BlobVariantReaderAndWriter<>)) return true;
            return false;
        }

        public string GenerateField(FieldInfo fi)
        {
            var valueType = fi.FieldType.GetGenericArguments()[0];
            var variantType = fi.FieldType.GetGenericTypeDefinition();
            if (variantType == typeof(BlobVariantReaderAndWriter<>))
                return $"public {VariantInterfaceNamespace}.SerializedVariantRW<{valueType.FullName}> {fi.Name};";
            if (variantType == typeof(BlobVariantReader<>))
                return $"public {VariantInterfaceNamespace}.SerializedVariantRO<{valueType.FullName}> {fi.Name};";
            return $"public {VariantInterfaceNamespace}.SerializedVariantWO<{valueType.FullName}> {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"{fi.Name}.Allocate(ref builder, ref data.{fi.Name}, Self, tree);";
        }
    }
}

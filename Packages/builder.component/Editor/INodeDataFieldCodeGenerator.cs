using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Editor
{
    public interface INodeDataFieldCodeGenerator
    {
        bool ShouldGenerate(FieldInfo fi);
        string GenerateField(FieldInfo fi);
        string GenerateBuild(FieldInfo fi);
    }

    public class DefaultNodeDataFieldCodeGenerator : INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            return true;
        }

        public string GenerateField(FieldInfo fi)
        {
            return $"public {fi.FieldType.FullName} {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"data.{fi.Name} = {fi.Name};";
        }
    }

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
            return fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(BlobString);
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
}

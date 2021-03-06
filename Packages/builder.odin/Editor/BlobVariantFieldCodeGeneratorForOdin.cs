using System;
using System.Reflection;
using System.Text;
using EntitiesBT.Core;
using EntitiesBT.Variant;

namespace EntitiesBT.Editor
{
    public class BlobVariantFieldCodeGeneratorForOdin : INodeDataFieldCodeGenerator
    {
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
            var variantType = fi.FieldType.GetGenericTypeDefinition();
            var valueType = fi.FieldType.GetGenericArguments()[0];
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("[OdinSerialize, NonSerialized]");
            stringBuilder.Append("        ");
            string variantInterfaceName = null;
            if (variantType == typeof(BlobVariantReaderAndWriter<>))
                variantInterfaceName = typeof(ISerializedVariantReaderAndWriter<>).Name;
            else if (variantType == typeof(BlobVariantReader<>))
                variantInterfaceName = typeof(IVariantReader<>).Name;
            else if (variantType == typeof(BlobVariantWriter<>))
                variantInterfaceName = typeof(IVariantWriter<>).Name;
            else
                throw new NotImplementedException($"Invalid type of variant {variantType}");
            // remove `1 from generic name
            variantInterfaceName = variantInterfaceName.Substring(0, variantInterfaceName.Length - 2);

            stringBuilder.AppendLine($"public EntitiesBT.Variant.{variantInterfaceName}<{valueType.FullName}> {fi.Name};");
            return stringBuilder.ToString();
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"{fi.Name}.Allocate(ref builder, ref data.{fi.Name}, Self, tree);";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EntitiesBT.Components.Odin;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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
            var attributes = new List<string>
            {
                nameof(OdinSerializeAttribute)
                , nameof(NonSerializedAttribute)
                , nameof(HideReferenceObjectPickerAttribute)
            };
            Type serializedVariantType = null;
            if (variantType == typeof(BlobVariantReaderAndWriter<>))
            {
                serializedVariantType = typeof(OdinSerializedVariantReaderAndWriter<>);
            }
            else if (variantType == typeof(BlobVariantReader<>))
            {
                attributes.Add(nameof(HideLabelAttribute));
                serializedVariantType = typeof(OdinSerializedVariantReader<>);
            }
            else if (variantType == typeof(BlobVariantWriter<>))
            {
                attributes.Add(nameof(HideLabelAttribute));
                serializedVariantType = typeof(OdinSerializedVariantWriter<>);
            }
            else
            {
                throw new NotImplementedException($"Invalid type of variant {variantType}");
            }
            // remove `1 from generic name
            var serializedVariantName = serializedVariantType.Name.Substring(0, serializedVariantType.Name.Length - 2);
            var serializedVariantNamespace = serializedVariantType.Namespace;
            var serializedVariantTypeFullname = $"{serializedVariantNamespace}.{serializedVariantName}<{valueType.FullName}>";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"[{string.Join(", ", attributes)}]");
            stringBuilder.AppendLine($"        public {serializedVariantTypeFullname} {fi.Name}");
            stringBuilder.AppendLine($"            = new {serializedVariantTypeFullname}();");
            return stringBuilder.ToString();
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"{fi.Name}.Allocate(ref builder, ref data.{fi.Name}, Self, tree);";
        }
    }
}
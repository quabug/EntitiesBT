using System;
using Nuwa;
using Nuwa.Blob;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class BlobVariantBuilder : PlainDataBuilder<BlobVariant>
    {
        [SerializeField, HideInInspector] private string _variantTypeName;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        private IVariant _variant;

        public override void Build(BlobBuilder builder, ref BlobVariant data)
        {
            _variant.Allocate(ref builder, ref data);
        }

        public class Factory : DynamicBuilderFactory<BlobVariantBuilder>
        {
            public override bool IsValid(Type dataType)
            {
                return dataType.IsGenericType && (IsBlobVariantRO(dataType) || IsBlobVariantRW(dataType));
            }

            public override object Create(Type dataType)
            {
                var valueType = dataType.GenericTypeArguments[0];
                var variantType = typeof(IVariant<>);
                if (IsBlobVariantRO(dataType)) variantType = typeof(IVariantReader<>);
                else if (IsBlobVariantRW(dataType)) variantType = typeof(IVariantWriter<>);
                return new BlobVariantBuilder { _variantTypeName = variantType.MakeGenericType(valueType).AssemblyQualifiedName };
            }

            private bool IsBlobVariantRO(Type dataType)
            {
                return dataType.GetGenericTypeDefinition() == typeof(BlobVariantRO<>);
            }

            private bool IsBlobVariantRW(Type dataType)
            {
                return dataType.GetGenericTypeDefinition() == typeof(BlobVariantRW<>);
            }
        }
    }
}
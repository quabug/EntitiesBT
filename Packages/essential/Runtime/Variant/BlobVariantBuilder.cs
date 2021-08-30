using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Nuwa;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class BlobVariantROBuilder : Builder<BlobVariant>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;
        [SerializeField, HideInInspector] internal bool IsOptional;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", NullableVariable = nameof(IsOptional))]
        private IVariantReader _variant;

        public override void Build(BlobBuilder builder, ref BlobVariant data)
        {
            if (_variant != null) _variant.Allocate(ref builder, ref data);
        }

        public class Factory : DynamicBuilderFactory<BlobVariantROBuilder>
        {
            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantRO<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                var valueType = dataType.GenericTypeArguments[0];
                var variantType = typeof(IVariantReader<>);
                var isOptional = fieldInfo.GetCustomAttributes<OptionalAttribute>().Any();
                return new BlobVariantROBuilder { _variantTypeName = variantType.MakeGenericType(valueType).AssemblyQualifiedName, IsOptional = isOptional };
            }
        }
    }

    [Serializable]
    public class BlobVariantWOBuilder : Builder<BlobVariant>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;
        [SerializeField, HideInInspector] internal bool IsOptional;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", NullableVariable = nameof(IsOptional))]
        private IVariantWriter _variant;

        public override void Build(BlobBuilder builder, ref BlobVariant data)
        {
            _variant.Allocate(ref builder, ref data);
        }

        public class Factory : DynamicBuilderFactory<BlobVariantWOBuilder>
        {
            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantWO<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                var valueType = dataType.GenericTypeArguments[0];
                var variantType = typeof(IVariantWriter<>);
                var isOptional = fieldInfo.GetCustomAttributes<OptionalAttribute>().Any();
                return new BlobVariantWOBuilder { _variantTypeName = variantType.MakeGenericType(valueType).AssemblyQualifiedName, IsOptional = isOptional };
            }
        }
    }

    [Serializable]
    public class BlobVariantRWBuilder : Builder<BlobVariantRW>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;
        [SerializeField, HideInInspector] internal bool IsOptional;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", NullableVariable = nameof(IsOptional))]
        private IVariantReaderAndWriter _variant;

        public override unsafe void Build(BlobBuilder builder, ref BlobVariantRW data)
        {
            var metaDataPtr = _variant.Allocate(ref builder, ref data.Reader);
            data.Writer.VariantId = data.Reader.VariantId;

            // HACK: set meta data of writer as same as reader's
            ref var writerMetaPtr = ref Utilities.ToBlobPtr<byte>(ref data.Writer.MetaDataOffsetPtr);
            builder.SetPointer(ref writerMetaPtr, ref UnsafeUtility.AsRef<byte>(metaDataPtr.ToPointer()));
        }
    }

    [Serializable]
    public class BlobVariantLinkedRWBuilder : Builder<BlobVariantRW>
    {
        [SerializeField] private bool _isLinked = true;
        [HideIf(nameof(_isLinked)), SerializeField, UnboxSingleProperty, UnityDrawProperty] private BlobVariantROBuilder _reader;
        [HideIf(nameof(_isLinked)), SerializeField, UnboxSingleProperty, UnityDrawProperty] private BlobVariantWOBuilder _writer;
        [HideIf(nameof(_isLinked), false), SerializeField, UnboxSingleProperty, UnityDrawProperty] private BlobVariantRWBuilder _readerAndWriter;

        public override void Build(BlobBuilder builder, ref BlobVariantRW data)
        {
            if (_isLinked)
            {
                _readerAndWriter.Build(builder, ref data);
            }
            else
            {
                _reader.Build(builder, ref data.Reader);
                _writer.Build(builder, ref data.Writer);
            }
        }

        public class Factory : DynamicBuilderFactory<BlobVariantLinkedRWBuilder>
        {
            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantRW<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                var valueType = dataType.GenericTypeArguments[0];
                var isOptional = fieldInfo.GetCustomAttributes<OptionalAttribute>().Any();
                return new BlobVariantLinkedRWBuilder
                {
                    _reader = new BlobVariantROBuilder { _variantTypeName = typeof(IVariantReader<>).MakeGenericType(valueType).AssemblyQualifiedName, IsOptional = isOptional},
                    _writer = new BlobVariantWOBuilder { _variantTypeName = typeof(IVariantWriter<>).MakeGenericType(valueType).AssemblyQualifiedName, IsOptional = isOptional },
                    _readerAndWriter = new BlobVariantRWBuilder { _variantTypeName = typeof(IVariantReaderAndWriter<>).MakeGenericType(valueType).AssemblyQualifiedName, IsOptional = isOptional },
                };
            }
        }
    }
}
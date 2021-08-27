using System;
using Nuwa;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class BlobVariantROBuilder : PlainDataBuilder<BlobVariant>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        private IVariantReader _variant;

        public override void Build(BlobBuilder builder, ref BlobVariant data)
        {
            _variant.Allocate(ref builder, ref data);
        }

        public class Factory : DynamicBuilderFactory<BlobVariantROBuilder>
        {
            public override bool IsValid(Type dataType)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantRO<>);
            }

            public override object Create(Type dataType)
            {
                var valueType = dataType.GenericTypeArguments[0];
                var variantType = typeof(IVariantReader<>);
                return new BlobVariantROBuilder { _variantTypeName = variantType.MakeGenericType(valueType).AssemblyQualifiedName };
            }
        }
    }

    [Serializable]
    public class BlobVariantWOBuilder : PlainDataBuilder<BlobVariant>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        private IVariantWriter _variant;

        public override void Build(BlobBuilder builder, ref BlobVariant data)
        {
            _variant.Allocate(ref builder, ref data);
        }

        public class Factory : DynamicBuilderFactory<BlobVariantWOBuilder>
        {
            public override bool IsValid(Type dataType)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantWO<>);
            }

            public override object Create(Type dataType)
            {
                var valueType = dataType.GenericTypeArguments[0];
                var variantType = typeof(IVariantWriter<>);
                return new BlobVariantWOBuilder { _variantTypeName = variantType.MakeGenericType(valueType).AssemblyQualifiedName };
            }
        }
    }

    [Serializable]
    public class BlobVariantRWBuilder : PlainDataBuilder<BlobVariantRW>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
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
    public class BlobVariantLinkedRWBuilder : PlainDataBuilder<BlobVariantRW>
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
            public override bool IsValid(Type dataType)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantRW<>);
            }

            public override object Create(Type dataType)
            {
                var valueType = dataType.GenericTypeArguments[0];
                return new BlobVariantLinkedRWBuilder
                {
                    _reader = new BlobVariantROBuilder { _variantTypeName = typeof(IVariantReader<>).MakeGenericType(valueType).AssemblyQualifiedName },
                    _writer = new BlobVariantWOBuilder { _variantTypeName = typeof(IVariantWriter<>).MakeGenericType(valueType).AssemblyQualifiedName },
                    _readerAndWriter = new BlobVariantRWBuilder { _variantTypeName = typeof(IVariantReaderAndWriter<>).MakeGenericType(valueType).AssemblyQualifiedName },
                };
            }
        }
    }
}
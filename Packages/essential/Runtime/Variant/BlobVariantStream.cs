using System;
using System.Linq;
using System.Reflection;
using Blob;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Nuwa;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Variant
{
    public class BlobVariantStream
    {
        [NotNull] private readonly IBlobStream _stream;
        private readonly int _idPosition;
        private readonly int _offsetPosition;
        private readonly int _patchPosition;

        public BlobVariantStream([NotNull] IBlobStream stream)
        {
            _stream = stream;
            stream.EnsureDataSize<BlobVariant>();
            _idPosition = stream.DataPosition;
            _patchPosition = stream.PatchPosition;
            stream.WriteValue(new BlobVariant().VariantId);
            _offsetPosition = stream.DataPosition;
        }

        public void SetVariantId(int id)
        {
            _stream.DataPosition = _idPosition;
            _stream.WriteValue(id);
        }

        public void SetVariantValue<T>(T value) where T : unmanaged
        {
            _stream.DataPosition = _offsetPosition;
            _stream.WritePatchOffset();
            _stream.DataPosition = _patchPosition;
            _stream.WriteValue(value);
        }

        public void SetVariantValue<T>(IBuilder<T> builder) where T : unmanaged
        {
            _stream.DataPosition = _offsetPosition;
            _stream.WritePatchOffset();
            _stream.DataPosition = _patchPosition;
            builder.Build(_stream);
        }

        public void SetVariantOffset(int offset)
        {
            _stream.DataPosition = _offsetPosition;
            _stream.WriteValue(offset);
        }
    }

    [Serializable]
    public class BlobVariantROBuilder : Nuwa.Blob.Builder<BlobVariant>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;
        [SerializeField, HideInInspector] internal bool IsOptional;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", NullableVariable = nameof(IsOptional))]
        private IVariantReader _variant;

        public override object PreviewValue { get => _variant.PreviewValue; set => throw new NotImplementedException(); }

        protected override void BuildImpl(IBlobStream stream, UnsafeBlobStreamValue<BlobVariant> value)
        {
            _variant?.Allocate(new BlobVariantStream(stream));
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
    public class BlobVariantWOBuilder : Nuwa.Blob.Builder<BlobVariant>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;
        [SerializeField, HideInInspector] internal bool IsOptional;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", NullableVariable = nameof(IsOptional))]
        private IVariantWriter _variant;

        public override object PreviewValue { get => _variant.PreviewValue; set => throw new NotImplementedException(); }

        protected override void BuildImpl(IBlobStream stream, UnsafeBlobStreamValue<BlobVariant> value)
        {
            _variant.Allocate(new BlobVariantStream(stream));
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
    public class BlobVariantRWBuilder : Nuwa.Blob.Builder<BlobVariantRW>
    {
        [SerializeField, HideInInspector] internal string _variantTypeName;
        [SerializeField, HideInInspector] internal bool IsOptional;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_variantTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", NullableVariable = nameof(IsOptional))]
        private IVariantReaderAndWriter _variant;

        public override object PreviewValue { get => _variant.PreviewValue; set => throw new NotImplementedException(); }

        protected override unsafe void BuildImpl(IBlobStream stream, UnsafeBlobStreamValue<BlobVariantRW> value)
        {
            _variant.Allocate(new BlobVariantStream(stream));
            value.Value.Writer.VariantId = value.Value.Reader.VariantId;
            // HACK: set meta data of writer as same as reader's
            fixed (void* writerDataPtr = &value.Value.Writer.MetaDataOffsetPtr)
            fixed (void* readerDataPtr = &value.Value.Reader.MetaDataOffsetPtr)
            {
                value.Value.Writer.MetaDataOffsetPtr =
                    value.Value.Reader.MetaDataOffsetPtr + (int)((long)writerDataPtr - (long)readerDataPtr);
            }
        }
    }

    [Serializable]
    public class BlobVariantLinkedRWBuilder : Nuwa.Blob.Builder<BlobVariantRW>
    {
        [SerializeField] private bool _isLinked = true;
        [HideIf(nameof(_isLinked)), SerializeField, UnboxSingleProperty, UnityDrawProperty] private BlobVariantROBuilder _reader;
        [HideIf(nameof(_isLinked)), SerializeField, UnboxSingleProperty, UnityDrawProperty] private BlobVariantWOBuilder _writer;
        [HideIf(nameof(_isLinked), false), SerializeField, UnboxSingleProperty, UnityDrawProperty] private BlobVariantRWBuilder _readerAndWriter;

        public override object PreviewValue { get => _isLinked ? _readerAndWriter.PreviewValue : _reader.PreviewValue; set => throw new NotImplementedException(); }

        protected override void BuildImpl(IBlobStream stream, UnsafeBlobStreamValue<BlobVariantRW> value)
        {
            if (_isLinked)
            {
                _readerAndWriter.Build(stream);
            }
            else
            {
                _reader.Build(stream);
                _writer.Build(stream);
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
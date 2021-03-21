using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components.Odin
{
    public class OdinSerializedVariantReaderAndWriter<T> : ISerializedVariantReaderAndWriter<T> where T : unmanaged
    {
        [field: SerializeField] public bool IsLinked { get; private set; } = true;

#if UNITY_EDITOR
        private IEnumerable<Type> _genericReaderAndWriter => Variant.Utilities.VARIANT_READER_AND_WRITER_TYPES.Value.ToOdinVariantType<T>();
        [field: TypeFilter(nameof(_genericReaderAndWriter))]
#endif
        [field: OdinSerialize, NonSerialized, ShowIf(nameof(IsLinked))]
        public IVariantReaderAndWriter<T> ReaderAndWriter { get; private set; } = new LocalVariant.ReaderAndWriter<T>();

#if UNITY_EDITOR
        private IEnumerable<Type> _genericReader => Variant.Utilities.VARIANT_READER_TYPES.Value.ToOdinVariantType<T>();
        [field: TypeFilter(nameof(_genericReader))]
#endif
        [field: OdinSerialize, NonSerialized, HideIf(nameof(IsLinked))]
        public IVariantReader<T> Reader { get; private set; } = new LocalVariant.Reader<T>();

#if UNITY_EDITOR
        private IEnumerable<Type> _genericWriter => Variant.Utilities.VARIANT_WRITER_TYPES.Value.ToOdinVariantType<T>();
        [field: TypeFilter(nameof(_genericWriter))]
#endif
        [field: OdinSerialize, NonSerialized, HideIf(nameof(IsLinked))]
        public IVariantWriter<T> Writer { get; private set; } = new OdinNodeVariant.Writer<T>();
    }

    public class OdinSerializedVariantReader<T> where T : unmanaged
    {
#if UNITY_EDITOR
        private IEnumerable<Type> _genericReader => Variant.Utilities.VARIANT_READER_TYPES.Value.ToOdinVariantType<T>();
        [field: TypeFilter(nameof(_genericReader))]
#endif
        [field: OdinSerialize, NonSerialized]
        public IVariantReader<T> Reader { get; private set; } = new LocalVariant.Reader<T>();

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariantReader<T> blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            return Reader.Allocate(ref builder, ref blobVariant, self, tree);
        }
    }

    public class OdinSerializedVariantWriter<T> where T : unmanaged
    {
#if UNITY_EDITOR
        private IEnumerable<Type> _genericWriter => Variant.Utilities.VARIANT_WRITER_TYPES.Value.ToOdinVariantType<T>();
        [field: TypeFilter(nameof(_genericWriter))]
#endif
        [field: OdinSerialize, NonSerialized]
        public IVariantWriter<T> Writer { get; private set; } = new OdinNodeVariant.Writer<T>();

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariantWriter<T> blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            return Writer.Allocate(ref builder, ref blobVariant, self, tree);
        }
    }
}
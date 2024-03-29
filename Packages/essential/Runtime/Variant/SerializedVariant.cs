using System;
using Nuwa;
using EntitiesBT.Core;
using Nuwa.Blob;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class SerializedVariantRW<T> : ISerializedVariantRW<T> where T : unmanaged
    {
        [UnityEngine.SerializeField]
        private bool _isLinked = true;
        public bool IsLinked => _isLinked;

        [UnityEngine.SerializeReference]
        [HideIf(nameof(_isLinked), false)]
        [SerializeReferenceDrawer(TypeRestrictBySiblingProperty = nameof(ReaderAndWriter), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2")]
        private object _readerAndWriter;
        public IVariantReaderAndWriter<T> ReaderAndWriter => (IVariantReaderAndWriter<T>)_readerAndWriter;

        [UnityEngine.SerializeReference]
        [HideIf(nameof(_isLinked))]
        [SerializeReferenceDrawer(TypeRestrictBySiblingProperty = nameof(Reader), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2")]
        private object _reader;
        public IVariantReader<T> Reader => (IVariantReader<T>)_reader;

        [UnityEngine.SerializeReference]
        [HideIf(nameof(_isLinked))]
        [SerializeReferenceDrawer(TypeRestrictBySiblingProperty = nameof(Writer), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2")]
        private object _writer;
        public IVariantWriter<T> Writer => (IVariantWriter<T>)_writer;

        public bool IsNull() => _isLinked ? _readerAndWriter == null : (_reader == null || _writer == null);
    }

    [Serializable]
    public class SerializedVariantRO<T> : IVariantReader<T> where T : unmanaged
    {
        [UnityEngine.SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingProperty = nameof(Reader), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2")]
        private object _reader;
        public IVariantReader<T> Reader => (IVariantReader<T>)_reader;

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Reader.Allocate(ref builder, ref blobVariant);
        }

        public object PreviewValue => Reader.PreviewValue;

        public bool IsNull() => _reader == null;
    }

    [Serializable]
    public class SerializedVariantWO<T> : IVariantWriter<T> where T : unmanaged
    {
        [UnityEngine.SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingProperty = nameof(Writer), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2")]
        private object _writer;
        public IVariantWriter<T> Writer => (IVariantWriter<T>)_writer;

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Writer.Allocate(ref builder, ref blobVariant);
        }

        public object PreviewValue => Writer.PreviewValue;

        public bool IsNull() => _writer == null;
    }
}
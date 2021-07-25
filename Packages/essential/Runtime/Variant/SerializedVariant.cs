using EntitiesBT.Attributes;

namespace EntitiesBT.Variant
{
    [System.Serializable]
    public class SerializedVariantRW<T> where T : unmanaged
    {
        [UnityEngine.SerializeField]
        private bool _isLinked = true;
        public bool IsLinked => _isLinked;

        [UnityEngine.SerializeReference]
        [HideIf(nameof(_isLinked), false)]
        [SerializeReferenceDrawer, SerializeReferenceDrawerPropertyBaseType(nameof(ReaderAndWriter))]
        private object _readerAndWriter;
        public IVariantReaderAndWriter<T> ReaderAndWriter => (IVariantReaderAndWriter<T>)_readerAndWriter;

        [UnityEngine.SerializeReference]
        [HideIf(nameof(_isLinked))]
        [SerializeReferenceDrawer, SerializeReferenceDrawerPropertyBaseType(nameof(Reader))]
        private object _reader;
        public IVariantReader<T> Reader => (IVariantReader<T>)_reader;

        [UnityEngine.SerializeReference]
        [HideIf(nameof(_isLinked))]
        [SerializeReferenceDrawer, SerializeReferenceDrawerPropertyBaseType(nameof(Writer))]
        private object _writer;
        public IVariantWriter<T> Writer => (IVariantWriter<T>)_writer;
    }

    [System.Serializable]
    public class SerializedVariantRO<T> where T : unmanaged
    {
        [UnityEngine.SerializeReference]
        [SerializeReferenceDrawer, SerializeReferenceDrawerPropertyBaseType(nameof(Reader))]
        private object _reader;
        public IVariantReader<T> Reader => (IVariantReader<T>)_reader;
    }

    [System.Serializable]
    public class SerializedVariantWO<T> where T : unmanaged
    {
        [UnityEngine.SerializeReference]
        [SerializeReferenceDrawer, SerializeReferenceDrawerPropertyBaseType(nameof(Writer))]
        private object _writer;
        public IVariantWriter<T> Writer => (IVariantWriter<T>)_writer;
    }
}
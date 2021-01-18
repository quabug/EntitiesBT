namespace EntitiesBT.Variant
{
    public interface ISerializedReaderAndWriter<T> where T : unmanaged
    {
        public bool IsLinked { get; }
        public IVariantReaderAndWriter<T> ReaderAndWriter { get; }
        public IVariantReader<T> Reader { get; }
        public IVariantWriter<T> Writer { get; }
    }
}
namespace EntitiesBT.Variant
{
    public interface ISerializedVariantReaderAndWriter<T> where T : unmanaged
    {
        bool IsLinked { get; }
        IVariantReaderAndWriter<T> ReaderAndWriter { get; }
        IVariantReader<T> Reader { get; }
        IVariantWriter<T> Writer { get; }
    }
}
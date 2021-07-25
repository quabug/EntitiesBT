namespace EntitiesBT.Variant
{
    public interface ISerializedVariantRW<out T> where T : unmanaged
    {
        bool IsLinked { get; }
        IVariantReaderAndWriter<T> ReaderAndWriter { get; }
        IVariantReader<T> Reader { get; }
        IVariantWriter<T> Writer { get; }
    }
}
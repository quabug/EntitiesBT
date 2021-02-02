using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public interface ISerializedReaderAndWriterVariant<T> where T : unmanaged
    {
        bool IsLinked { get; }
        IVariantReaderAndWriter<T> ReaderAndWriter { get; }
        IVariantReader<T> Reader { get; }
        IVariantWriter<T> Writer { get; }
    }
}
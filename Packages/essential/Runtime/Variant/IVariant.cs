using System;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public interface IVariant
    {
        // return: pointer of meta data
        IntPtr Allocate(
            ref BlobBuilder builder
          , ref BlobVariant blobVariant
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        );
    }

    public interface IVariantReader : IVariant {}
    public interface IVariantWriter : IVariant {}
    public interface IVariantReaderAndWriter : IVariant {}
    public interface IVariant<out T> : IVariant where T : unmanaged {}
    public interface IVariantReader<out T> : IVariantReader, IVariant<T> where T : unmanaged {}
    public interface IVariantWriter<out T> : IVariantWriter, IVariant<T> where T : unmanaged {}
    public interface IVariantReaderAndWriter<out T> : IVariantReaderAndWriter, IVariant<T> where T : unmanaged {}

    public static partial class VariantExtension
    {
        [CanBeNull] public static Type FindValueType([NotNull] this IVariant variant)
        {
            var type = variant.GetType();
            return type.GetInterface(typeof(IVariant<int>).Name)?.GenericTypeArguments[0];
        }
    }
}

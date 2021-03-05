using System;
using EntitiesBT.Core;
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

    public interface IVariant<out T> : IVariant where T : unmanaged {}
    public interface IVariantReader<out T> : IVariant<T> where T : unmanaged {}
    public interface IVariantWriter<out T> : IVariant<T> where T : unmanaged {}
    public interface IVariantReaderAndWriter<out T> : IVariant<T> where T : unmanaged {}
}

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

    public interface IVariant<T> : IVariant where T : unmanaged {}
    public interface IVariantReader<T> : IVariant<T> where T : unmanaged {}
    public interface IVariantWriter<T> : IVariant<T> where T : unmanaged {}
    public interface IVariantReaderAndWriter<T> : IVariant<T> where T : unmanaged {}
}

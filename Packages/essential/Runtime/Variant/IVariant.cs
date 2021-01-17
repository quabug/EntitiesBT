using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public interface IVariant
    {
        // return: pointer of meta data
        unsafe void* Allocate(
            ref BlobBuilder builder
          , ref BlobVariant blobVariant
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        );
    }

    public interface IVariantReader<T> : IVariant where T : unmanaged {}
    public interface IVariantWriter<T> : IVariant where T : unmanaged {}
    public interface IVariantReaderAndWriter<T> : IVariant where T : unmanaged {}
}

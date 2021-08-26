using EntitiesBT.Variant;
using Nuwa.Blob;

namespace EntitiesBT.Component
{
    public class BlobVariantROBuilder<T> : PlainDataBuilder<BlobVariantRO<T>> where T : unmanaged {}
}
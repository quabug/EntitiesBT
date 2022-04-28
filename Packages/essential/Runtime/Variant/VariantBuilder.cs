using System;
using Blob;

namespace EntitiesBT.Variant
{
    public class VariantBuilder<T> : Builder<T> where T : unmanaged
    {
        static unsafe VariantBuilder()
        {
            if (sizeof(T) != sizeof(BlobVariant)) throw new ArgumentException($"invalid generic type {typeof(T)}");
        }

        private readonly IVariant _variant;
        public VariantBuilder(IVariant variant) => _variant = variant;
        
        protected override void BuildImpl(IBlobStream stream)
        {
            _variant.Allocate(new BlobVariantStream(stream));
        }
    }
}
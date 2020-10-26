using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public struct BlobVariant
    {
        public int VariantId;
        public int MetaDataOffsetPtr;

        [Pure]
        public ref TValue Value<TValue>() where TValue : struct =>
            ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref MetaDataOffsetPtr).Value;

        [Pure]
        public IEnumerable<ComponentType> GetComponentAccessList() =>
            VariantRegisters.GetComponentAccess(VariantId)(ref this);
    }
}

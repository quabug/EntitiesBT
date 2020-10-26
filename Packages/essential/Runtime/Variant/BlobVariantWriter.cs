using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public struct BlobVariantWriter<T> : IRuntimeComponentAccessor where T : struct
    {
        internal BlobVariant Value;

        public void Write<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            VariantRegisters<T>.GetWriter<TNodeBlob, TBlackboard>(Value.VariantId)(ref Value, index, ref blob, ref bb, value);
        }

        public IEnumerable<ComponentType> AccessTypes => Value.GetComponentAccessList();
    }
}

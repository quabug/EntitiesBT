using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public struct BlobVariantReader<T> : IRuntimeComponentAccessor where T : struct
    {
        internal BlobVariant Value;

        public T Read<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return VariantRegisters<T>.TryGetValue<TNodeBlob, TBlackboard>(Value.VariantId, out var reader)
                ? reader(ref Value, index, ref blob, ref bb)
                : VariantRegisters<T>.GetRefReader<TNodeBlob, TBlackboard>(Value.VariantId)(ref Value, index, ref blob, ref bb)
            ;
        }

        public IEnumerable<ComponentType> AccessTypes =>
            Value.GetComponentAccessList().Select(t => ComponentType.ReadOnly(t.TypeIndex))
        ;
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlobVariantRO<T> : IRuntimeComponentAccessor where T : unmanaged
    {
        public BlobVariant Value;

        public T Read<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return Value.ReadWithRefFallback<T, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
        }

        public IEnumerable<ComponentType> AccessTypes =>
            Value.GetComponentAccessList().Select(t => ComponentType.ReadOnly(t.TypeIndex))
        ;

        public bool IsValid => Value.MetaDataOffsetPtr > 0;
    }
}

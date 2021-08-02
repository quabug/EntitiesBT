using System.Collections.Generic;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlobVariantWO<T> : IRuntimeComponentAccessor where T : unmanaged
    {
        internal BlobVariant Value;

        public void Write<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Value.WriteWithRefFallback(index, ref blob, ref bb, value);
        }

        public IEnumerable<ComponentType> AccessTypes => Value.GetComponentAccessList();

        public bool IsValid => Value.MetaDataOffsetPtr > 0;
    }
}

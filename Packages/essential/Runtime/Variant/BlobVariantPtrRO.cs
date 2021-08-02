using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlobVariantPtrRO : IRuntimeComponentAccessor
    {
        internal BlobVariant Value;

        public IntPtr GetPointer<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return Value.ReadOnlyPtrWithReadWriteFallback(index, ref blob, ref bb);
        }

        public IEnumerable<ComponentType> AccessTypes =>
            Value.GetComponentAccessList().Select(t => ComponentType.ReadOnly(t.TypeIndex))
        ;
    }
}

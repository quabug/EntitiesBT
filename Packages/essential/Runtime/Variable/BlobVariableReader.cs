using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariableReader<T> : IRuntimeComponentAccessor where T : unmanaged
    {
        public int VariableId;
        public int OffsetPtr;
        
        [Pure]
        public unsafe ref TValue Value<TValue>() where TValue : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(OffsetPtr == 0)
                throw new InvalidOperationException("The accessed BlobPtr hasn't been allocated.");
#endif
            fixed (int* thisPtr = &OffsetPtr)
            {
                return ref UnsafeUtility.AsRef<TValue>((byte*)thisPtr + OffsetPtr);
            }
        }

        public IEnumerable<ComponentType> ComponentAccessList => VariableReaderRegisters<T>.GetComponentAccess(VariableId)(ref this);
        
        [Pure]
        public T Read<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return VariableReaderRegisters<T>.Read<TNodeBlob, TBlackboard>(VariableId)(ref this, index, ref blob, ref bb);
        }
    }
}

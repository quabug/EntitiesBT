using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariable<T> : IRuntimeComponentAccessor where T : struct
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

        public IEnumerable<ComponentType> ComponentAccessList => VariableRegisters<T>.GetComponentAccess(VariableId)(ref this);
        
        [Pure]
        public T GetData<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return VariableRegisters<T>.GetData<TNodeBlob, TBlackboard>(VariableId)(ref this, index, ref blob, ref bb);
        }
        
        [Pure]
        public unsafe ref T GetDataRef<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            // NOTE: error CS8170: Struct members cannot return 'this' or other instance members by reference
            // return ref VariableRegisters<T>.GetDataRef(VariableId)(ref this, index, blob, bb);
            var ptr = UnsafeUtility.AddressOf(ref VariableRegisters<T>.GetDataRef<TNodeBlob, TBlackboard>(VariableId)(ref this, index, ref blob, ref bb));
            return ref UnsafeUtility.AsRef<T>(ptr);
        }
    }
}

using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariable<T> : IRuntimeComponentAccessor where T : struct
    {
        public int VariableId;
        public int OffsetPtr;
        
        public unsafe ref TValue Value<TValue>() where TValue : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(OffsetPtr == 0)
                throw new InvalidOperationException("The accessed BlobPtr hasn't been allocated.");
#endif
            fixed (int* thisPtr = &OffsetPtr)
            {
                return ref UnsafeUtilityEx.AsRef<TValue>((byte*)thisPtr + OffsetPtr);
            }
        }

        public IEnumerable<ComponentType> ComponentAccessList => VariableRegisters<T>.GetComponentAccess(VariableId)(ref this);
        
        
        public T GetData(int index, INodeBlob blob, IBlackboard bb)
        {
            return VariableRegisters<T>.GetData(VariableId)(ref this, index, blob, bb);
        }
        
        public unsafe ref T GetDataRef(int index, INodeBlob blob, IBlackboard bb)
        {
            // NOTE: error CS8170: Struct members cannot return 'this' or other instance members by reference
            // return ref VariableRegisters<T>.GetDataRef(VariableId)(ref this, index, blob, bb);
            var ptr = UnsafeUtility.AddressOf(ref VariableRegisters<T>.GetDataRef(VariableId)(ref this, index, blob, bb));
            return ref UnsafeUtilityEx.AsRef<T>(ptr);
        }
    }
}

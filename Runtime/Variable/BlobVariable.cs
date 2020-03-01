using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Variable
{
    public struct BlobVariable<T> where T : struct
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
        
        public T GetData(int index, INodeBlob blob, IBlackboard bb)
        {
            return VariableRegisters<T>.GET_DATA_FUNCS[VariableId](ref this, index, blob, bb);
        }
        
        public unsafe ref T GetDataRef(int index, INodeBlob blob, IBlackboard bb)
        {
            var ptr = UnsafeUtility.AddressOf(ref VariableRegisters<T>.GET_DATA_REF_FUNCS[VariableId](ref this, index, blob, bb));
            return ref UnsafeUtilityEx.AsRef<T>(ptr);
        }
    }
}

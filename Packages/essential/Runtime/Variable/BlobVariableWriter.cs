using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariableWriter<T> : IRuntimeComponentAccessor where T : unmanaged
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

        public IEnumerable<ComponentType> ComponentAccessList => throw new NotImplementedException();//VariableReaderRegisters<T>.GetComponentAccess(VariableId)(ref this);

        public void Write<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            throw new NotImplementedException();
            // return VariableReaderRegisters<T>.Read<TNodeBlob, TBlackboard>(VariableId)(ref this, index, ref blob, ref bb);
        }
    }
}

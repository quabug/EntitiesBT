using System.Collections.Generic;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariable
    {
        public int VariableId;
        public int MetaDataOffsetPtr;

        [Pure]
        public ref TValue Value<TValue>() where TValue : struct =>
            ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref MetaDataOffsetPtr).Value;

        [Pure]
        public IEnumerable<ComponentType> GetComponentAccessList() =>
            VariableRegisters.GetComponentAccess(VariableId)(ref this);
    }
}

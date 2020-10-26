using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariableWriter<T> : IRuntimeComponentAccessor where T : struct
    {
        internal BlobVariable Value;

        public void Write<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            VariableRegisters<T>.GetWriter<TNodeBlob, TBlackboard>(Value.VariableId)(ref Value, index, ref blob, ref bb, value);
        }

        public IEnumerable<ComponentType> AccessTypes => Value.GetComponentAccessList();
    }
}

using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public struct BlobVariableReader<T> : IRuntimeComponentAccessor where T : struct
    {
        internal BlobVariable Value;

        public T Read<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return VariableRegisters<T>.GetReader<TNodeBlob, TBlackboard>(Value.VariableId)(ref Value, index, ref blob, ref bb);
        }

        public IEnumerable<ComponentType> AccessTypes =>
            Value.GetComponentAccessList().Select(t => ComponentType.ReadOnly(t.TypeIndex))
        ;
    }
}

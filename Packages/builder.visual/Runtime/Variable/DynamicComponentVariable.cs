using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Builder.Visual
{
    public static class DynamicComponentVariableProperty
    {
        [Serializable]
        public class Reader<T> : IVariablePropertyReader<T> where T : unmanaged
        {
            private readonly ulong _stableHash;
            private readonly int _offset;

            public Reader(ulong stableHash, int offset)
            {
                _stableHash = stableHash;
                _offset = offset;
            }

            public void Allocate(ref BlobBuilder builder, ref BlobVariable blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariable.VariableId = GuidHashCode(ComponentVariableProperty.GUID);
                builder.Allocate(ref blobVariable, new ComponentVariableProperty.DynamicComponentData{StableHash = _stableHash, Offset = _offset});
            }
        }
    }
}

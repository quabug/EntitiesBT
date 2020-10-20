using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [Serializable]
    public class DynamicComponentVariablePropertyReader<T> : IVariablePropertyReader<T> where T : unmanaged
    {
        private readonly bool _useRef;
        private readonly ulong _stableHash;
        private readonly int _offset;

        public DynamicComponentVariablePropertyReader(ulong stableHash, int offset, bool useRef = false)
        {
            _useRef = useRef;
            _stableHash = stableHash;
            _offset = offset;
        }

        public void Allocate(ref BlobBuilder builder, ref BlobVariableReader<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            blobVariable.VariableId = _useRef
                 ? ComponentVariablePropertyReader<T>.DYNAMIC_ID
                 : ComponentVariablePropertyReader<T>.COPYTOLOCAL_ID
             ;
            if (_useRef) builder.Allocate(ref blobVariable, new ComponentVariablePropertyReader<T>.DynamicComponentData{StableHash = _stableHash, Offset = _offset});
            else builder.Allocate(ref blobVariable, new ComponentVariablePropertyReader<T>.CopyToLocalComponentData{StableHash = _stableHash, Offset = _offset, LocalValue = default});
        }
    }
}

using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [Serializable]
    public class DynamicComponentVariablePropertyReader<T> : VariablePropertyReader<T> where T : unmanaged
    {
        private readonly bool _useRef;
        private readonly ulong _stableHash;
        private readonly int _offset;

        public override int VariablePropertyTypeId => _useRef
            ? ComponentVariablePropertyReader<T>.DYNAMIC_ID
            : ComponentVariablePropertyReader<T>.COPYTOLOCAL_ID
        ;

        public DynamicComponentVariablePropertyReader(ulong stableHash, int offset, bool useRef = false)
        {
            _useRef = useRef;
            _stableHash = stableHash;
            _offset = offset;
        }

        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariableReader<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            if (_useRef) builder.Allocate(ref blobVariable, new ComponentVariablePropertyReader<T>.DynamicComponentData{StableHash = _stableHash, Offset = _offset});
            else builder.Allocate(ref blobVariable, new ComponentVariablePropertyReader<T>.CopyToLocalComponentData{StableHash = _stableHash, Offset = _offset, LocalValue = default});
        }
    }
}

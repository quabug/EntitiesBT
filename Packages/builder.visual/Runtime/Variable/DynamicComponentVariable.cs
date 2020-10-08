using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [Serializable]
    public class DynamicComponentVariableProperty<T> : VariableProperty<T> where T : unmanaged
    {
        private readonly bool _useRef;
        private readonly ulong _stableHash;
        private readonly int _offset;

        public override int VariablePropertyTypeId => _useRef
            ? ComponentVariableProperty<T>.DYNAMIC_ID
            : ComponentVariableProperty<T>.COPYTOLOCAL_ID
        ;

        public DynamicComponentVariableProperty(ulong stableHash, int offset, bool useRef = false)
        {
            _useRef = useRef;
            _stableHash = stableHash;
            _offset = offset;
        }

        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            if (_useRef) builder.Allocate(ref blobVariable, new ComponentVariableProperty<T>.DynamicComponentData{StableHash = _stableHash, Offset = _offset});
            else builder.Allocate(ref blobVariable, new ComponentVariableProperty<T>.CopyToLocalComponentData{StableHash = _stableHash, Offset = _offset, LocalValue = default});
        }
    }
}

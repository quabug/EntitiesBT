using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTSubTree : BTNode, INodeDataBuilder
    {
        [SerializeField] private BTNode _tree;
        
        public override BehaviorNodeType NodeType => BehaviorNodeType.Action; // cannot have children
        public override int NodeId => 0;
        public override int Size => 0;
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders) {}
        public override INodeDataBuilder Self => _tree;
    }
}

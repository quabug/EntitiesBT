using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTSequence : BTNode<SequenceNode>
    {
        [Tooltip("Enable this will re-evaluate node state from first child until running node instead of skip to running node directly.")]
        [SerializeField] private bool _recursiveResetStatesBeforeTick = default;

        public override INodeDataBuilder Self => _recursiveResetStatesBeforeTick
            ? new BTVirtualDecorator<RecursiveResetStateNode>(this)
            : base.Self
        ;
    }
}

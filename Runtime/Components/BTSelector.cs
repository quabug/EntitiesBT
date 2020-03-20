using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTSelector : BTNode<SelectorNode>
    {
        [Tooltip("Enable this will re-evaluate node state from first child instead of skip to running node directly.")]
        [SerializeField] private bool _recursiveResetStatesBeforeTick = default;

        protected override INodeDataBuilder SelfImpl => _recursiveResetStatesBeforeTick
            ? new BTVirtualDecorator<RecursiveResetStateNode>(this)
            : base.SelfImpl
        ;
    }
}

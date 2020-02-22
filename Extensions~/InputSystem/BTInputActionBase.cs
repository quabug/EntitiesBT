using System;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputActionBase<TNode> : BTNode<TNode> where TNode : struct, IInputActionNodeData
    {
        [SerializeField] private string _actionId;
        
        protected override void Build(ref TNode data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.ActionId = Guid.Parse(_actionId);
        }

        public override INodeDataBuilder Self => new BTVirtualDecorator<RunOnMainThreadNode>(this);
    }

    public interface IInputActionNodeData : INodeData
    {
        Guid ActionId { get; set; }
    }
}

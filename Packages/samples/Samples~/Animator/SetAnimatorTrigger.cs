using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class SetAnimatorTrigger : BTNode<SetAnimatorTriggerNode>
    {
        public string TriggerName;

        protected override unsafe void Build(void* dataPtr, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __) =>
            ((SetAnimatorTriggerNode*) dataPtr)->Value = Animator.StringToHash(TriggerName);

        protected override INodeDataBuilder SelfImpl => new BTVirtualDecorator<RunOnMainThreadNode>(this);
    }
    
    [BehaviorNode("EF8A0D43-DEA1-4D31-953C-77CD0BD8E26C")]
    public struct SetAnimatorTriggerNode : INodeData
    {
        public int Value;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
        {
            var animator = blackboard.GetObject<Animator>();
            animator.SetTrigger(Value);
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}

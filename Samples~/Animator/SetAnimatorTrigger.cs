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

        public override INodeDataBuilder Self => new BTVirtualDecorator<RunOnMainThreadNode>(this);
    }
    
    [BehaviorNode("EF8A0D43-DEA1-4D31-953C-77CD0BD8E26C")]
    public struct SetAnimatorTriggerNode : INodeData
    {
        public int Value;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<Animator>();
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var animator = blackboard.GetData<Animator>();
            animator.SetTrigger(blob.GetNodeData<SetAnimatorTriggerNode>(index).Value);
            return NodeState.Success;
        }
    }
}

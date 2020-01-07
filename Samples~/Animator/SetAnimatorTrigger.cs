using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class SetAnimatorTrigger : BTNode<SetAnimatorTriggerNode, SetAnimatorTriggerNode.Data>
    {
        public string TriggerName;

        public override unsafe void Build(void* dataPtr) =>
            ((SetAnimatorTriggerNode.Data*) dataPtr)->Value = Animator.StringToHash(TriggerName);
    }
    
    [BehaviorNode("EF8A0D43-DEA1-4D31-953C-77CD0BD8E26C")]
    public class SetAnimatorTriggerNode
    {
        public static readonly ComponentType[] Types = {ComponentType.ReadWrite<Animator>()};
        
        public struct Data : INodeData
        {
            public int Value;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var animator = blackboard.GetData<Animator>();
            animator.SetTrigger(blob.GetNodeData<Data>(index).Value);
            return NodeState.Success;
        }
    }
}

using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEditor.Animations;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class SetAnimatorTrigger : BTNode<SetAnimatorTriggerNode, SetAnimatorTriggerNode.Data>
    {
        public string TriggerName;
        public override unsafe void Build(void* dataPtr) =>
            ((SetAnimatorTriggerNode.Data*) dataPtr)->Value = Animator.StringToHash(TriggerName);
    }
    
    public class SetAnimatorTriggerNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int Value;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var animator = (Animator)blackboard[typeof(Animator)];
            animator.SetTrigger(blob.GetNodeData<Data>(index).Value);
            return NodeState.Success;
        }
    }
}

using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEditor.Animations;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class SetAnimatorTrigger : BTNode
    {
        public string TriggerName;
        public override IBehaviorNode BehaviorNode => new SetAnimatorTriggerNode();
        public override unsafe int Size => sizeof(SetAnimatorTriggerNode.Data);
        public override unsafe void Build(void* dataPtr) =>
            ((SetAnimatorTriggerNode.Data*) dataPtr)->Value = Animator.StringToHash(TriggerName);
    }
    
    public class SetAnimatorTriggerNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int Value;
        }

        public void Reset(VirtualMachine vm, int index, IBlackboard blackboard) {}

        public NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            var animator = (Animator)blackboard[typeof(Animator)];
            animator.SetTrigger(vm.GetNodeData<Data>(index).Value);
            return NodeState.Success;
        }
    }
}

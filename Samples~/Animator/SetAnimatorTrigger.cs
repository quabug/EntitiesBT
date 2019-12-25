using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEditor.Animations;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class SetAnimatorTrigger : BTNode
    {
        public Animator Animator;
        
        public string TriggerName;
        public override IBehaviorNode BehaviorNode => new SetAnimatorTriggerNode(Animator);
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
        
        private readonly Animator _animator;
        public SetAnimatorTriggerNode(Animator animator)
        {
            _animator = animator;
        }

        public void Reset(VirtualMachine vm, int index) {}

        public NodeState Tick(VirtualMachine vm, int index)
        {
            _animator.SetTrigger(vm.GetNodeData<Data>(index).Value);
            return NodeState.Success;
        }
    }
}

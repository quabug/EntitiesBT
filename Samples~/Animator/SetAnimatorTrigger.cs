using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEditor.Animations;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class SetAnimatorTrigger : BTNode<SetAnimatorTriggerNode.Data>
    {
        public string TriggerName;
        public override int NodeId => SetAnimatorTriggerNode.Id;

        public override unsafe void Build(void* dataPtr) =>
            ((SetAnimatorTriggerNode.Data*) dataPtr)->Value = Animator.StringToHash(TriggerName);
    }
    
    public static class SetAnimatorTriggerNode
    {
        public static int Id = 50;

        static SetAnimatorTriggerNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public int Value;
        }

        static void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var animator = (Animator)blackboard[typeof(Animator)];
            animator.SetTrigger(blob.GetNodeData<Data>(index).Value);
            return NodeState.Success;
        }
    }
}

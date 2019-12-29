using System;
using EntitiesBT.Components;
using EntitiesBT.Core;
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
        public static readonly int Id = new Guid("EF8A0D43-DEA1-4D31-953C-77CD0BD8E26C").GetHashCode();

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
            var animator = blackboard.GetData<Animator>();
            animator.SetTrigger(blob.GetNodeData<Data>(index).Value);
            return NodeState.Success;
        }
    }
}

using Blob;
using EntitiesBT.Core;
using Nuwa.Blob;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    [BehaviorNode("EF8A0D43-DEA1-4D31-953C-77CD0BD8E26C")]
    public struct SetAnimatorTriggerNode : INodeData
    {
        [CustomBuilder(typeof(AnimatorTriggerNameBuilder))] public int Value;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
        {
            var animator = blackboard.GetObject<Animator>();
            animator.SetTrigger(Value);
            return NodeState.Success;
        }
    }

    public class AnimatorTriggerNameBuilder : Nuwa.Blob.Builder<int>
    {
        public string TriggerName;

        protected override void BuildImpl(IBlobStream stream)
        {
            stream.WriteValue(Animator.StringToHash(TriggerName));
        }
    }
}

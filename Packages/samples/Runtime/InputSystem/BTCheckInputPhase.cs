using System;
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Extensions.InputSystem.InputExtensions;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTCheckInputPhase : BTInputActionBase<CheckInputPhaseNode>
    {
        public UnityEngine.InputSystem.InputActionPhase Phase;

        protected override void Build(ref CheckInputPhaseNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            data.Phase = Phase;
        }
    }

    [BehaviorNode("64804C8C-8B45-4FB1-A14F-968781178D7A")]
    public struct CheckInputPhaseNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        public UnityEngine.InputSystem.InputActionPhase Phase;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var phase = GetInputActionPhase<CheckInputPhaseNode, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
            return phase == Phase ? NodeState.Success : NodeState.Failure;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
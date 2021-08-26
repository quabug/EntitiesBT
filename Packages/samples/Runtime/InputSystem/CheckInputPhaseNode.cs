using System;
using EntitiesBT.Core;
using Nuwa.Blob;
using static EntitiesBT.Extensions.InputSystem.InputExtensions;

namespace EntitiesBT.Extensions.InputSystem
{
    [BehaviorNode("64804C8C-8B45-4FB1-A14F-968781178D7A")]
    public struct CheckInputPhaseNode : IInputActionNodeData
    {
        [field: CustomBuilder(typeof(InputActionGuidBuilder))] public Guid ActionId { get; set; }
        public UnityEngine.InputSystem.InputActionPhase Phase;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var phase = GetInputActionPhase<CheckInputPhaseNode, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
            return phase == Phase ? NodeState.Success : NodeState.Failure;
        }
    }
}
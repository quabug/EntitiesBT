using System;
using EntitiesBT.Core;
using static EntitiesBT.Extensions.InputSystem.InputExtensions;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTIsInputActionTriggered : BTInputActionBase<IsInputActionTriggeredNode> {}

    [BehaviorNode("EF8EAB7A-2D0B-443C-B2BC-125A6A0CF1ED")]
    public struct IsInputActionTriggeredNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var isTriggered = IsInputActionTriggered<IsInputActionTriggeredNode, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
            return isTriggered ? NodeState.Success : NodeState.Failure;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}

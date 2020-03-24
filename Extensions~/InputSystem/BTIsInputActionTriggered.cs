using System;
using EntitiesBT.Core;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTIsInputActionTriggered : BTInputActionBase<IsInputActionTriggeredNode> {}

    [BehaviorNode("EF8EAB7A-2D0B-443C-B2BC-125A6A0CF1ED")]
    public struct IsInputActionTriggeredNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }

        [ReadOnly(typeof(InputActionAssetComponent))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var isTriggered = bb.IsInputActionTriggered<IsInputActionTriggeredNode>(index, blob);
            return isTriggered ? NodeState.Success : NodeState.Failure;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}

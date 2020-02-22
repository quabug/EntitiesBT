using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputTriggerd : BTInputActionBase<InputTriggerNode> {}

    [BehaviorNode("EF8EAB7A-2D0B-443C-B2BC-125A6A0CF1ED")]
    public struct InputTriggerNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        
        public static readonly ComponentType[] Types = { ComponentType.ReadOnly<InputActionAssetComponent>() };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            return bb.IsInputActionTriggered<InputTriggerNode>(index, blob) ? NodeState.Success : NodeState.Failure;
        }
    }
}

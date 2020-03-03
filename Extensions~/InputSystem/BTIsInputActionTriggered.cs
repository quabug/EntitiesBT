using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTIsInputActionTriggered : BTInputActionBase<IsInputActionTriggeredNode> {}

    [BehaviorNode("EF8EAB7A-2D0B-443C-B2BC-125A6A0CF1ED")]
    public struct IsInputActionTriggeredNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<InputActionAssetComponent>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var isTriggered = bb.IsInputActionTriggered<IsInputActionTriggeredNode>(index, blob);
            return isTriggered ? NodeState.Success : NodeState.Failure;
        }
    }
}

using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputAimPosition : BTInputActionBase<InputAimPositionNode> {}

    [BehaviorTreeComponent]
    public struct BTInputAimPositionData : IComponentData
    {
        public float2 Value;
    }

    [Serializable, BehaviorNode("2B2517A4-1A55-4CF4-8A49-61DDFD1168B1")]
    public struct InputAimPositionNode : IInputActionNodeData
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
        public Guid ActionId { get; set; }
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<BTInputAimPositionData>();
            yield return ComponentType.ReadOnly<InputActionAssetComponent>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputAimPositionNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            bb.GetDataRef<BTInputAimPositionData>().Value = inputValue.Value;
            return NodeState.Success;
        }
    }

    public class InputAimPositionDebugView : BTDebugView<InputAimPositionNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
#endif
        public float2 AimPosition;
        
        public override void Tick()
        {
            base.Tick();
            AimPosition = Blackboard.GetData<BTInputAimPositionData>().Value;
        }
    }
}

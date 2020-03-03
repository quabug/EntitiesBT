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
    public class BTInputMove : BTInputActionBase<InputMoveNode> {}

    [BehaviorTreeComponent]
    public struct BTInputMoveData : IComponentData
    {
        public float2 Value;
    }

    [Serializable, BehaviorNode("21CF9C9B-2BD4-4336-BFEF-4671060D1BD9")]
    public struct InputMoveNode : IInputActionNodeData
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
        public Guid ActionId { get; set; }
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<BTInputMoveData>();
            yield return ComponentType.ReadOnly<InputActionAssetComponent>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputMoveNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            bb.GetDataRef<BTInputMoveData>().Value = inputValue.Value;
            return NodeState.Success;
        }
    }

    public class InputMoveDebugView : BTDebugView<InputMoveNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
#endif
        public float2 Move;
        
        public override void Tick()
        {
            base.Tick();
            Move = Blackboard.GetData<BTInputMoveData>().Value;
        }
    }
}

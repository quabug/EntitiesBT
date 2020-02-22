using System;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputLook : BTInputActionBase<InputLookNode> {}

    [BehaviorTreeComponent]
    public struct BTInputLookData : IComponentData
    {
        public float2 Value;
    }

    [Serializable, BehaviorNode("2EBE8CF0-CFF1-436A-AB2F-1E9DABF3A1A3")]
    public struct InputLookNode : IInputActionNodeData
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
        public Guid ActionId { get; set; }
        
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadWrite<BTInputLookData>()
          , ComponentType.ReadOnly<InputActionAssetComponent>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputLookNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            bb.GetDataRef<BTInputLookData>().Value = inputValue.Value;
            return NodeState.Success;
        }
    }

    public class InputLookDebugView : BTDebugView<InputLookNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
#endif
        public float2 Look;
        
        public override void Tick()
        {
            base.Tick();
            Look = Blackboard.GetData<BTInputLookData>().Value;
        }
    }
}

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
        public Guid ActionId { get; set; }
        
        [ReadWrite(typeof(BTInputAimPositionData))]
        [ReadOnly(typeof(InputActionAssetComponent))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = index.ReadInputActionValue<InputAimPositionNode, Vector2, TNodeBlob, TBlackboard>(ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            bb.GetDataRef<BTInputAimPositionData>().Value = inputValue.Value;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }

    public class InputAimPositionDebugView : BTDebugView<InputAimPositionNode>
    {
        public float2 AimPosition;
        
        public override void Tick()
        {
            base.Tick();
            AimPosition = Blackboard.Value.GetData<BTInputAimPositionData>().Value;
        }
    }
}

using System.Collections.Generic;
using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Extensions.InputSystem;
using EntitiesBT.Extensions.UnityMovement;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Samples
{
    public class BTInputMoveToRotation : BTNode<InputMoveToRotationNode> {}

    [StructLayout(LayoutKind.Explicit), BehaviorNode("2164B3CA-C12E-4C86-9F80-F45A99124FAD")]
    public struct InputMoveToRotationNode : INodeData
    {
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<BTInputMoveData>();
            yield return ComponentType.ReadWrite<BTTransformRotationData>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var move = bb.GetData<BTInputMoveData>().Value;
            if (math.lengthsq(move) <= math.FLT_MIN_NORMAL) return NodeState.Success;
            
            var direction = quaternion.LookRotationSafe(new float3(move.x, 0, move.y), math.up());
            bb.GetDataRef<BTTransformRotationData>().Value = direction;
            return NodeState.Success;
        }
    }
}

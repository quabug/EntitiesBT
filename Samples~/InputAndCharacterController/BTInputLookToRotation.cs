using System.Collections.Generic;
using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Extensions.InputSystem;
using EntitiesBT.Extensions.UnityMovement;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Samples
{
    // public class BTInputLookToRotation : BTNode<InputLookToRotationNode>
    // {
    //     public float RotateSpeed;
    //
    //     [SerializeReference, SerializeReferenceButton]
    //     public float2Property InputLookProperty;
    //     
    //     protected override void Build(ref InputLookToRotationNode data, ITreeNode<INodeDataBuilder>[] builders)
    //     {
    //         data.RotateSpeed = RotateSpeed;
    //     }
    // }
    //
    // [StructLayout(LayoutKind.Explicit), BehaviorNode("7896AD37-C525-4D4F-8ECC-BF375F34FF9A")]
    // public struct InputLookToRotationNode : INodeData
    // {
    //     public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
    //     {
    //         yield return ComponentType.ReadOnly<BTInputLookData>();
    //         yield return ComponentType.ReadWrite<BTTransformRotationData>();
    //     }
    //     
    //     public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
    //     {
    //         var look = bb.GetData<BTInputLookData>().Value;
    //         var direction = quaternion.LookRotation(new float3(look.x, 0, look.y), math.up());
    //         bb.GetDataRef<BTTransformRotationData>().Value = direction;
    //         return NodeState.Success;
    //     }
    // }
}

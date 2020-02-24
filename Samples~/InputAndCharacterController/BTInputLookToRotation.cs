using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Extensions.InputSystem;
using EntitiesBT.Extensions.UnityMovement;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Samples
{
    public class BTInputLookToRotation : BTNode<InputLookToRotationNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.MinValue(0)]
#endif
        public float RotateSpeed;
        //
        // protected override void Build(ref InputLookToRotationNode data, ITreeNode<INodeDataBuilder>[] builders)
        // {
        //     data.RotateSpeed = RotateSpeed;
        // }
    }

    [StructLayout(LayoutKind.Explicit), BehaviorNode("7896AD37-C525-4D4F-8ECC-BF375F34FF9A")]
    public struct InputLookToRotationNode : INodeData
    {
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadOnly<BTInputLookData>()
            , ComponentType.ReadWrite<BTTransformRotationData>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var look = bb.GetData<BTInputLookData>().Value;
            var direction = quaternion.LookRotation(new float3(look.x, 0, look.y), math.up());
            bb.GetDataRef<BTTransformRotationData>().Value = direction;
            return NodeState.Success;
        }
    }
}
